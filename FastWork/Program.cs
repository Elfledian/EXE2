using FastWork.Services.EmailService;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repo.Data;
using Repo.Entities;
using Repo.Repositories;
using Repo;
using Service.DTO;
using Service.Services.EmailService;
using Service.Services;
using Service;
using System.Text.Json.Serialization;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using FastWork.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
// Add logging first to capture all events
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug); // Ensure debug-level logs
});
builder.Services.AddEndpointsApiExplorer();

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<PayOSService>();
builder.Services.AddScoped<PaymentRepo>();
builder.Services.AddDbContext<TheShineDbContext>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection("PayOS"));

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<TheShineDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Authorization");
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(options =>
   {
       var issuer = builder.Configuration["Jwt:Issuer"];
       var audience = builder.Configuration["Jwt:Audience"];
       var key = builder.Configuration["Jwt:Key"];
       Console.WriteLine($"Middleware Config - Issuer: {issuer}, Audience: {audience}, Key: {key}");
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = issuer,
           ValidAudience = audience,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
           ClockSkew = TimeSpan.FromMinutes(5)
       };
       options.Events = new JwtBearerEvents
       {
           OnMessageReceived = context =>
           {
               var authHeader = context.Request.Headers["Authorization"].ToString();
               Console.WriteLine($"Received Authorization Header: {authHeader}");
               if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
               {
                   context.Token = authHeader.Substring("Bearer ".Length).Trim();
                   Console.WriteLine($"Extracted Token: {context.Token}");
               }
               return Task.CompletedTask;
           },
           OnAuthenticationFailed = context =>
           {
               Console.WriteLine($"Authentication Failed: {context.Exception.Message}");
               return Task.CompletedTask;
           }
       };
   });
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter the JWT token only (do not include 'Bearer' prefix)",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

// Seed roles on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await SeedData.SeedRoles(roleManager);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].ToString();
    Console.WriteLine($"Raw Authorization Header: {authHeader}");
    Console.WriteLine($"Header Length: {authHeader.Length}, Dots: {authHeader.Count(c => c == '.')}");
    await next();
    if (context.Response.StatusCode == 401)
    {
        Console.WriteLine($"401 Unauthorized for Path: {context.Request.Path}");
    }
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// Add middleware to log the request pipeline
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Pipeline at {DateTime.Now}: Method={context.Request.Method}, Path={context.Request.Path}, Query={context.Request.QueryString}, Headers={string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
    await next();
    if (context.Response.StatusCode == 404)
    {
        Console.WriteLine($"404 Not Found at {DateTime.Now}: Path={context.Request.Path}, RouteData={context.GetRouteData()?.Values}");
    }
});
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience) || string.IsNullOrWhiteSpace(key))
{
    throw new InvalidOperationException("JWT configuration is missing or invalid.");
}
Console.WriteLine($"Startup Config - Issuer: {issuer}, Audience: {audience}, Key: {key}");

// Add a fallback endpoint to catch unmatched routes
app.Map("/{**catchAll}", (HttpContext context) =>
{
    Console.WriteLine($"Unmatched Route at {DateTime.Now}: Path={context.Request.Path}, Method={context.Request.Method}, Query={context.Request.QueryString}");
    return Results.NotFound(new { Message = "Endpoint not found", Path = context.Request.Path, HasValue = context.Request.HasFormContentType });
});

app.MapControllers();
app.UseCors("AllowLocalhost5173");
// Add a health check endpoint to verify routing
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Time = DateTime.Now }));

var jwtConfig = builder.Configuration.GetSection("Jwt").Get<Dictionary<string, string>>();
Console.WriteLine("Jwt Config at Startup: " + (jwtConfig != null ? string.Join(", ", jwtConfig.Select(kvp => $"{kvp.Key}: {kvp.Value}")) : "null"));

app.Run();