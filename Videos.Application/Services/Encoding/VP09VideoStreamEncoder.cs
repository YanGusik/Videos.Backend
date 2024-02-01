using FFmpeg.NET;
using FFmpeg.NET.Events;
using Microsoft.Extensions.Logging;
using Videos.Application.Interfaces;
using Videos.Domain;

namespace Videos.Application.Services.Encoding
{
    [Obsolete]
    public class VP09VideoStreamEncoder
    {
        private readonly ILogger<VP09VideoStreamEncoder> _logger;
        private readonly IVideoDbContext _db;

        public VP09VideoStreamEncoder(ILogger<VP09VideoStreamEncoder> logger, IVideoDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<MetaData> GetMetaData(Video video)
        {
            if (video?.FileDetails?.Path == null) { throw new ArgumentNullException(nameof(video)); }

            var ffmpeg = new Engine("ffmpeg");
            var inputFile = new InputFile(video.FileDetails.Path);
            return await ffmpeg.GetMetaDataAsync(inputFile, new CancellationToken());
        }

        public async Task<MetaData> GetMetaData(string path)
        {
            var ffmpeg = new Engine("ffmpeg");
            var inputFile = new InputFile(path);
            return await ffmpeg.GetMetaDataAsync(inputFile, new CancellationToken());
        }

        public async Task Encode(Video video)
        {
            if (video?.FileDetails?.Path == null) { throw new ArgumentNullException(nameof(video)); }

            var inputFile = new InputFile(video.FileDetails.Path);
            var outputFile = new OutputFile(video.FileDetails.Path + "_encoded.mp4");

            var ffmpeg = new Engine("ffmpeg");
            ffmpeg.Data += OnData;
            ffmpeg.Progress += (object? sender, ConversionProgressEventArgs e) => { OnProgress(sender, e, video); };
            ffmpeg.Error += (object? sender, ConversionErrorEventArgs e) => { OnError(sender, e, video); };
            ffmpeg.Complete += (object? sender, ConversionCompleteEventArgs e) => { OnComplete(sender, e, video); };

            var options = new ConversionOptions();
            SetOptions(options);
            await ffmpeg.ConvertAsync(inputFile, outputFile, options, new CancellationToken());
        }

        private void SetOptions(ConversionOptions options)
        {
            options.VideoCodec = FFmpeg.NET.Enums.VideoCodec.libvpx__vp9;
            options.VideoFormat = FFmpeg.NET.Enums.VideoFormat.mp4;
            options.VideoSize = FFmpeg.NET.Enums.VideoSize.Custom;
        }

        private void OnData(object? sender, ConversionDataEventArgs e)
        {
            if (e.Input is MediaFile input)
            {
                _logger.LogDebug(string.Format("[{0} => {1}]: {2}", input.FileInfo.Name, e.Output.Name, e.Data));
            }
        }

        private void OnProgress(object? sender, ConversionProgressEventArgs e, Video video)
        {
            if (e.Input is MediaFile input)
            {
                video.ProcessingDetails.PartsTotal = e.TotalDuration.TotalSeconds;
                video.ProcessingDetails.PartsProcessed = e.ProcessedDuration.TotalSeconds;
                video.ProcessingDetails.TimeLeftMs = (e.TotalDuration - e.ProcessedDuration).TotalMilliseconds;
                video.ProcessingDetails.Status = VideoStatus.Processing;
                video.ProcessingDetails.Stage = VideoStage.Encoding;
                _db.Videos.Update(video); // TODO: я не понимаю, почему не трекает, без него, плохо или нет
                _db.SaveChanges();


                _logger.LogInformation(string.Format(
                    "[{0} => {1}]\n" +
                    "Bitrate: {0}\n" +
                    "Fps: {0}\n" +
                    "Frame: {0}\n" +
                    "ProcessedDuration: {0}\n" +
                    "Size: {0} kb\n" +
                    "TotalDuration: {0}\n\n" +
                    "Осталось: {0}\n", 
                    e.Input.MetaData.FileInfo.Name, e.Output.Name,
                    e.Bitrate,
                    e.Fps,
                    e.Frame,
                    e.ProcessedDuration,
                    e.SizeKb,
                    e.TotalDuration,
                    e.TotalDuration - e.ProcessedDuration
                    ));

                //_logger.LogInformation(string.Format("[{0} => {1}]", e.Input.MetaData.FileInfo.Name, e.Output.Name));
                //_logger.LogInformation(string.Format("Bitrate: {0}", e.Bitrate));
                //_logger.LogInformation(string.Format("Fps: {0}", e.Fps));
                //_logger.LogInformation(string.Format("Frame: {0}", e.Frame));
                //_logger.LogInformation(string.Format("ProcessedDuration: {0}", e.ProcessedDuration));
                //_logger.LogInformation(string.Format("Size: {0} kb", e.SizeKb));
                //_logger.LogInformation(string.Format("TotalDuration: {0}\n", e.TotalDuration));
                //_logger.LogInformation(string.Format("Осталось: {0}\n", e.TotalDuration - e.ProcessedDuration));
            }
            else throw new Exception("e.Input is not a MediaFile");
        }

        private void OnComplete(object? sender, ConversionCompleteEventArgs e, Video video)
        {
            if (e.Input is MediaFile input)
            {
                video.ProcessingDetails.Status = VideoStatus.Processed; 
                video.ProcessingDetails.TimeLeftMs = 0;
                video.ProcessingDetails.PartsProcessed = 0;
                video.ProcessingDetails.PartsTotal = 0;
                _db.Videos.Update(video); // TODO: я не понимаю, почему не трекает, без него, плохо или нет
                _db.SaveChanges();
                // TODO: next job (мне не нравится, что один класс и кодирует и пересылает работу на другой job,
                // возможно надо переименовать, потому что статус кодирования получаем, только из событий)

                _logger.LogInformation(string.Format("Completed conversion from {0} to {1}", input.FileInfo.Name, e.Output.Name));
            }
            else throw new Exception("e.Input is not a MediaFile");
        }

        private void OnError(object? sender, ConversionErrorEventArgs e, Video video)
        {
            if (e.Input is MediaFile input)
            {
                video.ProcessingDetails.Status = VideoStatus.Failed;
                video.ProcessingDetails.TimeLeftMs = 0;
                video.ProcessingDetails.PartsProcessed = 0;
                video.ProcessingDetails.PartsTotal = 0;
                _db.Videos.Update(video);
                _db.SaveChanges();
                _logger.LogError(string.Format("[{0} => {1}]: Error: {2}\n{3}", input.FileInfo.Name, e.Output.Name, e.Exception.ExitCode, e.Exception.InnerException));
                throw e.Exception;
            }
            else throw new Exception("e.Input is not a MediaFile");
        }
    }

}
