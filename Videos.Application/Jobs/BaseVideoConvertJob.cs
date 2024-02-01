using FFmpeg.NET.Events;
using FFmpeg.NET;
using Microsoft.Extensions.Logging;
using Videos.Domain;
using Videos.Application.Interfaces;

namespace Videos.Application.Jobs
{
    public abstract class BaseVideoConvertJob : FFmpegEventLogger
    {
        public abstract string Prefix { get; }
        public abstract string Quality { get; }
        protected readonly IVideoDbContext _db;

        public BaseVideoConvertJob(ILogger<BaseVideoConvertJob> logger, IVideoDbContext db) : base(logger)
        {
            _db = db;
        }

        public async Task Encode(Video video, CancellationToken cancellationToken = default)
        {
            await _db.Entry(video).ReloadAsync();
            if (video?.FileDetails?.Path == null) { throw new ArgumentNullException(nameof(video)); }

            var inputFile = new InputFile(video.FileDetails.Path);
            var outputFile = GetOutputFile(video.FileDetails.Path);

            var ffmpeg = new Engine("ffmpeg");
            ffmpeg.Data += OnDataLog;
            ffmpeg.Progress += (s, e) => { OnProgress(e, video); };
            ffmpeg.Error += (s, e) => { OnError(e, video); };
            ffmpeg.Complete += (s, e) => { OnComplete(e, video); };

            var options = await GetConversionOptions(video.FileDetails.Path);
            await ffmpeg.ConvertAsync(inputFile, outputFile, options, cancellationToken);
        }

        protected OutputFile GetOutputFile(string path)
        {
            var fileInfo = new FileInfo(path);
            var name = fileInfo.DirectoryName + "/" + Path.GetFileNameWithoutExtension(fileInfo.FullName) + Prefix;
            return new OutputFile(string.Format("{0}{1}", name, fileInfo.Extension));
        }

        protected abstract Task<ConversionOptions> GetConversionOptions(string path);

        protected void OnProgress(ConversionProgressEventArgs e, Video video)
        {
            if (e.Input is MediaFile input)
            {
                video.ProcessingDetails.PartsTotal = e.TotalDuration.TotalSeconds;
                video.ProcessingDetails.PartsProcessed = e.ProcessedDuration.TotalSeconds;
                video.ProcessingDetails.TimeLeftMs = (e.TotalDuration - e.ProcessedDuration).TotalMilliseconds;
                video.ProcessingDetails.Status = VideoStatus.Processing;
                video.ProcessingDetails.Stage = VideoStage.Encoding;
                _db.Videos.Update(video);
                _db.SaveChanges();

                OnProgressLog(e);
            }
            else throw new Exception("e.Input is not a MediaFile");
        }

        protected void OnComplete(ConversionCompleteEventArgs e, Video video)
        {
            if (video?.FileDetails?.Path == null)
                throw new ArgumentException("Video path is null");

            var path = video.FileDetails.Path;
            var efd = new EncodedFileDetails();
            var outputFile = GetOutputFile(path);
            efd.Name = outputFile.FileInfo.Name;
            efd.Path = outputFile.FileInfo.FullName;
            efd.Size = outputFile.FileInfo.Length;

            video.FileDetails.EncodedFiles.Add(Quality, efd);
            video.Processed();
            _db.Videos.Update(video);
            _db.SaveChanges();
            OnCompleteLog(e);
        }

        protected void OnError(ConversionErrorEventArgs e, Video video)
        {
            video.Error();
            _db.Videos.Update(video);
            _db.SaveChanges();
            OnErrorLog(e);
        }
    }
}
