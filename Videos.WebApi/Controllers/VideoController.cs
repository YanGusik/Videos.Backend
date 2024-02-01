using Microsoft.AspNetCore.Mvc;
using Videos.Application.Services;
using Video = Videos.Domain.Video;

namespace Videos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : BaseController
    {
        private VideoService _videoService;

        public VideoController(VideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet("view")]
        public IEnumerable<Video> Index()
        {
            return _videoService.GetVideos();
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(int id)
        {
            var video = await _videoService.GetVideo(id);
            return Ok(new { video });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string title)
        {
            using (var stream = file.OpenReadStream())
            {
                var video = await _videoService.Upload(stream, title, file.FileName, UserId);
                return Ok(new { video });
            }
        }
    }
}
