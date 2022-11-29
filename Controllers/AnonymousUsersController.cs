using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecretsSharingAPI.EF;

namespace SecretsSharingAPI.Controllers
{
    [ApiController]
    [Route("api/anonymous")]
    public class AnonymousUsersController : Controller
    {
        private readonly UserContext _user;
        public AnonymousUsersController(UserContext user)
        {
            _user = user;
        }

        [HttpGet("all-files")]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = await _user.Files.Select(f => f.Uri).ToListAsync();
            return Ok(files);
        }

        [HttpPost("download")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> DownloadFile([FromQuery] string link)
        {
            var file = await _user.Files.FirstOrDefaultAsync(f => f.Uri == link);
            if (file == null)
            {
                return BadRequest("File not found");
            }

            var content = file.DataFiles;

            if (file.IsOnceDowloaded)
            {
                _user.Files.Remove(file);
                _user.SaveChanges();
            }

            return File(content, "application/octet-stream", file.Name);
        }
    }
}
