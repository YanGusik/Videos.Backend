using Hangfire;
using Videos.Application.Interfaces;
using Videos.Application.Jobs;
using Videos.Domain;

namespace Videos.Application.Services
{
    public class VideoService
    {
        private readonly IVideoDbContext _dbContext;

        public VideoService(IVideoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Video> GetVideos()
        {
            return _dbContext.Videos.ToList();
        }

        public async Task<Video?> GetVideo(int id)
        {
            return await _dbContext.Videos.FindAsync(id);
        }

        public async Task<Video> Upload(Stream stream, string title, string fileName, int userId)
        {
            var file = await LocalFileService.SaveFileAsync(stream, fileName, userId);
            var processingDetails = new ProcessingDetails()
            {
                Status = VideoStatus.Uploaded
            };
            var video = new Video() { Title = title, ProcessingDetails = processingDetails };
            video.FileDetails = await VideoHelper.GetFileDetails(file.FullName);
            _dbContext.Videos.Add(video);
            await _dbContext.SaveChangesAsync();

            VideoProcessing(video);
            return video;
        }

        public void VideoProcessing(Video video)
        {
            var thumbailId = BackgroundJob.Enqueue<ThumbnailGenerateJob>(x => x.Generate(video, new CancellationToken()));
            var j420Id = BackgroundJob.ContinueJobWith<Video420ConvertJob>(thumbailId, x => x.Encode(video, new CancellationToken()));
            var j720Id = BackgroundJob.ContinueJobWith<Video720ConvertJob>(j420Id, x => x.Encode(video, new CancellationToken()));
            BackgroundJob.ContinueJobWith<Video1080ConvertJob>(j720Id, x => x.Encode(video, new CancellationToken()));
        }
    }
}
