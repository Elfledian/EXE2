using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repo.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    sub_items = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__categori__D54EE9B4DDD25EB7", x => x.category_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    file_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    file_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    file_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    file_data = table.Column<byte[]>(type: "longblob", nullable: true),
                    content_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    upload_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__files__07D884C68CA1CF9F", x => x.file_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    tag_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tags__4296A2B61A399BE7", x => x.tag_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    google_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    remember_me = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false),
                    terms_agreed = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false),
                    location = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, defaultValue: "Ho Chi Minh City"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    cvfileid = table.Column<Guid>(type: "char(36)", nullable: true),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__B9BE370FA57EB208", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_files_cvfileid",
                        column: x => x.cvfileid,
                        principalTable: "files",
                        principalColumn: "file_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "candidates",
                columns: table => new
                {
                    candidate_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "Chua nh?n vi?c"),
                    gender = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    education = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    income_range = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "1-3M"),
                    verified = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false),
                    featured = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__candidat__39BD4C18354A8933", x => x.candidate_id);
                    table.ForeignKey(
                        name: "FK__candidate__user___4CA06362",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    company_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    recruiter_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    company_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    website = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    logo_file_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__companie__3E267235299690E3", x => x.company_id);
                    table.ForeignKey(
                        name: "FK__companies__recru__52593CB8",
                        column: x => x.recruiter_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_companies_files",
                        column: x => x.logo_file_id,
                        principalTable: "files",
                        principalColumn: "file_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    message = table.Column<string>(type: "longtext", nullable: false),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__E059842F6357338B", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__notificat__user___2645B050",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    transaction_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    paid_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__payments__ED1FC9EAC80CA886", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK__payments__user__1332DBDC",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    rating_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    reviewer_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "longtext", nullable: true),
                    contributed_comment = table.Column<string>(type: "longtext", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ratings__D35B278B9864B1A5", x => x.rating_id);
                    table.ForeignKey(
                        name: "FK__ratings__reviewe__18EBB532",
                        column: x => x.reviewer_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recruiters",
                columns: table => new
                {
                    recruiter_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    company_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    scale = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    contact_phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    verified = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__recruite__42ABA257E9F06E1E", x => x.recruiter_id);
                    table.ForeignKey(
                        name: "FK__recruiter__user___59FA5E80",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    subscription_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    plan = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    subtitle = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    duration_days = table.Column<int>(type: "int", nullable: false),
                    is_activated = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    original_price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    features = table.Column<string>(type: "longtext", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__subscrip__863A7EC161176C70", x => x.subscription_id);
                    table.ForeignKey(
                        name: "FK__subscript__user__6A30C649",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "verifications",
                columns: table => new
                {
                    verification_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    blockchain_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    verified_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__verifica__24F179694FEA4951", x => x.verification_id);
                    table.ForeignKey(
                        name: "FK__verificat__user___1F98B2C1",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cv_details",
                columns: table => new
                {
                    cv_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    candidate_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    full_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    contact_email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    contact_phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    education = table.Column<string>(type: "longtext", nullable: true),
                    experience = table.Column<string>(type: "longtext", nullable: true),
                    skills = table.Column<string>(type: "longtext", nullable: true),
                    portfolio_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "longtext", nullable: true),
                    position = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    field = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    languages = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    desired_salary = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cv_detai__C36883E6681921B8", x => x.cv_id);
                    table.ForeignKey(
                        name: "FK__cv_detail__candi__31B762FC",
                        column: x => x.candidate_id,
                        principalTable: "candidates",
                        principalColumn: "candidate_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "work_history",
                columns: table => new
                {
                    history_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    candidate_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    content = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__work_his__096AA2E992B2BFDA", x => x.history_id);
                    table.ForeignKey(
                        name: "FK__work_hist__candi__367C1819",
                        column: x => x.candidate_id,
                        principalTable: "candidates",
                        principalColumn: "candidate_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    job_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    recruiter_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    company_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    category_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    position = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "longtext", nullable: false),
                    job_details = table.Column<string>(type: "longtext", nullable: true),
                    requirements = table.Column<string>(type: "longtext", nullable: true),
                    experience = table.Column<string>(type: "longtext", nullable: true),
                    benefits = table.Column<string>(type: "longtext", nullable: true),
                    salary = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    company_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    contact_phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    location = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, defaultValue: "Ho Chi Minh City"),
                    duration = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "Open"),
                    posted_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_urgent = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__jobs__6E32B6A5C5F2B37E", x => x.job_id);
                    table.ForeignKey(
                        name: "FK__jobs__category_i__778AC167",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__jobs__company_id__75A278F5",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id");
                    table.ForeignKey(
                        name: "FK__jobs__recruiter___74AE54BC",
                        column: x => x.recruiter_id,
                        principalTable: "recruiters",
                        principalColumn: "recruiter_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    application_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    job_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    candidate_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "Applied"),
                    about = table.Column<string>(type: "longtext", nullable: true),
                    about_me = table.Column<string>(type: "longtext", nullable: true),
                    interview_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    applied_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__applicat__3BCBDCF27B586E0C", x => x.application_id);
                    table.ForeignKey(
                        name: "FK__applicati__candi__03F0984C",
                        column: x => x.candidate_id,
                        principalTable: "candidates",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK__applicati__job_i__02FC7413",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    invitation_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    recruiter_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    candidate_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    job_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    message = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    sent_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__invitati__94B74D7C19DB176C", x => x.invitation_id);
                    table.ForeignKey(
                        name: "FK__invitatio__candi__0B91BA14",
                        column: x => x.candidate_id,
                        principalTable: "candidates",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK__invitatio__job_i__0C85DE4D",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id");
                    table.ForeignKey(
                        name: "FK__invitatio__recru__0A9D95DB",
                        column: x => x.recruiter_id,
                        principalTable: "recruiters",
                        principalColumn: "recruiter_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "job_tags",
                columns: table => new
                {
                    job_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    tag_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__job_tags__1A1BDC8E5B68F0DB", x => new { x.job_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK__job_tags__job_id__7A672E12",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__job_tags__tag_id__7B5B524B",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_applications_candidate_id",
                table: "applications",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "idx_applications_job_id",
                table: "applications",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_cvfileid",
                table: "AspNetUsers",
                column: "cvfileid");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E6164023178F8",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__B43B145FB0B35F61",
                table: "AspNetUsers",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__CCBDE7DC88483CA3",
                table: "AspNetUsers",
                column: "google_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__candidat__B9BE370E1C8D5C72",
                table: "candidates",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_categories_title",
                table: "categories",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "IX_companies_logo_file_id",
                table: "companies",
                column: "logo_file_id");

            migrationBuilder.CreateIndex(
                name: "UQ__companie__42ABA25659E2F898",
                table: "companies",
                column: "recruiter_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_cv_details_candidate_id",
                table: "cv_details",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_invitations_candidate_id",
                table: "invitations",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_invitations_job_id",
                table: "invitations",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_invitations_recruiter_id",
                table: "invitations",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "idx_job_tags_job_id",
                table: "job_tags",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_tags_tag_id",
                table: "job_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "idx_jobs_recruiter_id",
                table: "jobs",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_category_id",
                table: "jobs",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_company_id",
                table: "jobs",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "idx_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_payments_userid",
                table: "payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_ratings_reviewer_id",
                table: "ratings",
                column: "reviewer_id");

            migrationBuilder.CreateIndex(
                name: "UQ__recruite__B9BE370E94EF86F8",
                table: "recruiters",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tags__72E12F1B77600626",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__verifica__B9BE370EE289CFF9",
                table: "verifications",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_work_history_candidate_id",
                table: "work_history",
                column: "candidate_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applications");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "cv_details");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropTable(
                name: "job_tags");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "verifications");

            migrationBuilder.DropTable(
                name: "work_history");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "candidates");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "recruiters");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "files");
        }
    }
}
