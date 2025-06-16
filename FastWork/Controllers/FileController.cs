using System;
using System.IO;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Repo.Data;
using Repo.Entities;
using Service.Helper;
using File = Repo.Entities.File;
using System.Security.Claims;
using MySql.Data.MySqlClient;

namespace FastWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly TheShineDbContext _context; // Replace with your actual DbContext type
        private readonly UserManager<User> _userManager; // Assuming you have a User entity and UserManager

        public FileController(TheShineDbContext context, UserManager<User> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadSQLServer(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            var fileId = Guid.NewGuid();
            var fileName = file.FileName;
            var contentType = file.ContentType;
            var uploadDate = DateTime.UtcNow;

            var connectionString = _context.Database.GetDbConnection().ConnectionString;

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var cmdText = @"
        INSERT INTO files (file_id, file_name, file_type, content_type, file_data, upload_date)
        VALUES (@fileId, @fileName, @fileType, @contentType, @fileData, @uploadDate)";

            await using var cmd = new MySqlCommand(cmdText, connection);
            cmd.Parameters.AddWithValue("@fileId", fileId.ToString());
            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@fileType", contentType);
            cmd.Parameters.AddWithValue("@contentType", contentType);
            cmd.Parameters.Add("@fileData", MySqlDbType.LongBlob).Value = fileBytes;
            cmd.Parameters.AddWithValue("@uploadDate", uploadDate);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 1)
                return Ok(new { id = fileId });
            else
                return StatusCode(500, "Error inserting file");
        }

        //[HttpPost("uploadaaaa")]
        //public async Task<IActionResult> UploadFileAsyncaa([FromForm] FileUploadModel filee)
        //{
        //    var file = filee.File;
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded");

        //    byte[] fileBytes;
        //    using (var ms = new MemoryStream())
        //    {
        //        await file.CopyToAsync(ms);
        //        fileBytes = ms.ToArray();
        //    }

        //    var fileEntity = new File
        //    {
        //        FileId = Guid.NewGuid(),
        //        FileName = file.FileName,
        //        FileType = file.ContentType,
        //        ContentType = file.ContentType,
        //        FileData = fileBytes,
        //        UploadDate = DateTime.UtcNow
        //    };

        //    _context.Files.Add(fileEntity);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { id = fileEntity.FileId });
        //}

        public class FileUploadModel
        {
            public IFormFile File { get; set; }
        }
        //[HttpPost("uploadMySql")]
        //public async Task<IActionResult> UploadFileMySql(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded");

        //    byte[] fileBytes;
        //    using (var ms = new MemoryStream())
        //    {
        //        await file.CopyToAsync(ms);
        //        fileBytes = ms.ToArray();
        //    }

        //    var fileId = Guid.NewGuid();
        //    var fileName = file.FileName;
        //    var contentType = file.ContentType;
        //    var uploadDate = DateTime.UtcNow;

        //    var connectionString = _context.Database.GetDbConnection().ConnectionString;

        //    await using var connection = new MySqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    var cmdText = @"
        //INSERT INTO files (file_id, file_name, file_type, content_type, file_data, upload_date)
        //VALUES (@fileId, @fileName, @fileType, @contentType, @fileData, @uploadDate)";

        //    await using var cmd = new MySqlCommand(cmdText, connection);
        //    cmd.Parameters.AddWithValue("@fileId", fileId.ToString());
        //    cmd.Parameters.AddWithValue("@fileName", fileName);
        //    cmd.Parameters.AddWithValue("@fileType", contentType);
        //    cmd.Parameters.AddWithValue("@contentType", contentType);
        //    cmd.Parameters.Add("@fileData", MySqlDbType.LongBlob).Value = fileBytes;
        //    cmd.Parameters.AddWithValue("@uploadDate", uploadDate);

        //    var rowsAffected = await cmd.ExecuteNonQueryAsync();

        //    if (rowsAffected == 1)
        //        return Ok(new { id = fileId });
        //    else
        //        return StatusCode(500, "Error inserting file");
        //}


        // GET: api/File/download/{id}
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var file = await _context.Set<File>().FindAsync(id);
            if (file == null)
                return NotFound();
            return File(file.FileData, file.ContentType ?? "application/octet-stream", file.FileName);
        }

        [HttpGet("showPicture")]
        public async Task<IActionResult> GetImage(Guid picId)
        {
            try
            {
                var file = await _context.Files
                    .Where(f => f.FileId == picId)
                    .FirstOrDefaultAsync();

                if (file == null)
                {
                    return NotFound("File not found");
                }

                // Check if file is an image based on content type
                if (string.IsNullOrEmpty(file.ContentType) ||
                    !file.ContentType.StartsWith("image/"))
                {
                    return BadRequest("File is not an image");
                }

                return File(file.FileData, file.ContentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public ActionResult GetFiles()
        {
            var files = _context.Set<File>().ToList();
            return Ok(files);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            var file = await _context.Set<File>().FindAsync(id);
            if (file == null)
                return NotFound();
            return Ok(file);
        }
        [Authorize]
        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new
                {
                    Message = "Not authenticated",
                    ReceivedToken = Request.Headers["Authorization"],
                    AuthenticationType = User.Identity?.AuthenticationType,
                    Exception = HttpContext.Features.Get<Exception>()?.Message
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { UserId = userId });
        }

        [Authorize]
        [HttpPut("add-cv-file")]
        public async Task<IActionResult> AddCVToProfile([FromQuery] Guid cvId)
        {

            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;
            if (user == null)
            {
                return NotFound(new AppResponse<object>().SetErrorResponse("User", new[] { "User not found." }));
            }

            user.CvFileId = cvId;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new AppResponse<object>().SetErrorResponse("UpdateErrors", errors));
            }
            return Ok(new AppResponse<object>().SetSuccessResponse(null, "Message", "CV file updated successfully."));
        }
        [Authorize]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

    }
}
