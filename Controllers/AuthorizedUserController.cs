using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SecretsSharingAPI.EF;
using SecretsSharingAPI.Models;

namespace SecretsSharingAPI.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class AuthorizedUserController : Controller
    {
        private readonly UserContext _user;
        public AuthorizedUserController(UserContext user)
        {
            _user = user;
        }

        /// <summary>
        /// An authorized user can see their own files
        /// </summary>
        /// <returns>Returns OK response with files</returns>
        [HttpGet("user-files")]
        public async Task<IActionResult> GetAllUserFiles(string userEmail)
        {
            var user = await _user.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            var files = user.Files;
            return Ok(files);
        }

        /// <summary>
        /// After uploading, the user will receive the file URL
        /// </summary>
        /// <param name="file">file to upload</param>
        /// <param name="userEmail">User email from authorized page</param>
        /// <param name="isOnceDownloaded">Users should be able to specify if they want the file to be automatically deletedonce it’s downloaded</param>
        /// <returns>file URL</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, string userEmail, bool isOnceDownloaded)
        {
            var user = await _user.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            var fileUri = string.Empty;
            if (file != null)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var guidForUri = Convert.ToString(Guid.NewGuid());
                    fileUri = new Uri($"https://localhost:7100/api/{guidForUri.Substring(0, guidForUri.IndexOf('-'))}{fileExtension}").ToString();
                    var newFile = new Models.File()
                    {
                        UserId = user.Id,
                        Name = fileName,
                        FileType = fileExtension,
                        User = user,
                        Uri = fileUri,
                        IsOnceDowloaded = isOnceDownloaded
                    };

                    using (var target = new MemoryStream())
                    {
                        file.CopyTo(target);
                        newFile.DataFiles = target.ToArray();
                    }

                    _user.Files.Add(newFile);
                    _user.SaveChanges(true);
                }
                
            }
            return Ok(fileUri);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var fileToDelete = await _user.Files.FirstOrDefaultAsync(f => f.Id == fileId);
            if (fileToDelete == null)
            {
                return BadRequest("File not found");
            }

            _user.Files.Remove(fileToDelete);
            _user.SaveChanges();
            return Ok();
        }
    }
}
