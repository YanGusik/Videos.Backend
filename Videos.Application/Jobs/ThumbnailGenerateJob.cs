using FFmpeg.NET;
using FFmpeg.NET.Events;
using Microsoft.Extensions.Logging;
using System.Threading;
using Videos.Application.Interfaces;
using Videos.Application.Services;
using Videos.Domain;

namespace Videos.Application.Jobs
{
    public class ThumbnailGenerateJob : FFmpegEventLogger
    {
        private readonly IVideoDbContext _db;

        public ThumbnailGenerateJob(ILogger<BaseVideoConvertJob> logger, IVideoDbContext db) : base(logger)
        {
            _db = db;
        }

        public async Task Generate(Video video, CancellationToken cancellationToken = default)
        {
            await _db.Entry(video).ReloadAsync();
            if (video?.FileDetails?.Path == null) { throw new ArgumentNullException(nameof(video)); }

            var inputFile = new InputFile(video.FileDetails.Path);
            var outputFile = GetOutputFile(video.UserId);

            var ffmpeg = new Engine("ffmpeg");
            ffmpeg.Data += OnDataLog;
            ffmpeg.Progress += (s, e) => { OnProgressLog(e); };
            ffmpeg.Error += (s, e) => { OnErrorLog(e); };
            ffmpeg.Complete += (s, e) => { OnComplete(e, video); };

            await ffmpeg.GetThumbnailAsync(inputFile, outputFile, cancellationToken);
        }

        private void OnComplete(ConversionCompleteEventArgs e, Video video)
        {
            OnCompleteLog(e);
            video.FileDetails.ThumbnailPath = (e.Output as MediaFile)?.FileInfo.FullName;
        }

        private OutputFile GetOutputFile(int userId)
        {
            var dir = LocalFileService.GetThumbailDir(userId);
            var name = Path.GetRandomFileName() + ".jpg";
            var path = Path.Combine(dir, name);
            return new OutputFile(path);
        }
    }
}
