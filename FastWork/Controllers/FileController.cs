using Microsoft.AspNetCore.Mvc;
using Repo.Entities;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using File = Repo.Entities.File;

namespace FastWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly TheShineDbContext _context; // Replace with your actual DbContext type

        public FileController(TheShineDbContext context)
        {
            _context = context;
        }

        // POST: api/File/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var entity = new File
            {
                FileId = Guid.NewGuid(),
                FileName = file.FileName,
                FileType = Path.GetExtension(file.FileName),
                FileData = ms.ToArray(),
                ContentType = file.ContentType,
                UploadDate = DateTime.UtcNow
            };

            _context.Set<File>().Add(entity);
            await _context.SaveChangesAsync();

            var downloadUrl = Url.Action(nameof(Download), "File", new { id = entity.FileId }, Request.Scheme);

            return Ok(new { link = downloadUrl , id = entity.FileId});
        }

        // GET: api/File/download/{id}
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var file = await _context.Set<File>().FindAsync(id);
            if (file == null)
                return NotFound();

            return File(file.FileData, file.ContentType ?? "application/octet-stream", file.FileName);
        }
    }
}
