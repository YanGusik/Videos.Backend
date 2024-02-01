using FFmpeg.NET.Events;
using FFmpeg.NET;
using Microsoft.Extensions.Logging;
using Videos.Application.Interfaces;

namespace Videos.Application.Jobs
{
    public class FFmpegEventLogger
    {
        protected readonly ILogger _logger;

        public FFmpegEventLogger(ILogger<BaseVideoConvertJob> logger)
        {
            _logger = logger;
        }

        protected void OnDataLog(object? sender, ConversionDataEventArgs e)
        {
            if (e.Input is MediaFile input)
            {
                _logger.LogDebug(string.Format("[{0} => {1}]: {2}", input.FileInfo.Name, e.Output.Name, e.Data));
            }
        }

        protected void OnProgressLog(ConversionProgressEventArgs e)
        {
            if (e.Input is MediaFile input)
            {
                _logger.LogInformation(string.Format(
                    "[{0} => {1}]\n" +
                    "Bitrate: {2}\n" +
                    "Fps: {3}\n" +
                    "Frame: {4}\n" +
                    "ProcessedDuration: {5}\n" +
                    "Size: {6} kb\n" +
                    "TotalDuration: {7}\n" +
                    "LeftDuration: {8}\n",
                    e.Input.MetaData.FileInfo.Name, e.Output.Name,
                    e.Bitrate,
                    e.Fps,
                    e.Frame,
                    e.ProcessedDuration,
                    e.SizeKb,
                    e.TotalDuration,
                    e.TotalDuration - e.ProcessedDuration
                    ));
            }
            else throw new Exception("e.Input is not a MediaFile");
        }

        protected void OnCompleteLog(ConversionCompleteEventArgs e)
        {
            if (e.Input is MediaFile input)
            {
                _logger.LogInformation(string.Format("Completed conversion from {0} to {1}", input.FileInfo.Name, e.Output.Name));
            }
            else throw new Exception("e.Input is not a MediaFile");
        }

        protected void OnErrorLog(ConversionErrorEventArgs e)
        {
            if (e.Input is MediaFile input)
            {
                _logger.LogError(string.Format("[{0} => {1}]: Error: {2}\n{3}", input.FileInfo.Name, e.Output.Name, e.Exception.ExitCode, e.Exception.InnerException));
                throw e.Exception;
            }
            else throw new Exception("e.Input is not a MediaFile");
        }
    }
}
