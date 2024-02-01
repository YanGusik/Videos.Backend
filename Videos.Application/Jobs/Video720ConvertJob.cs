using FFmpeg.NET;
using Microsoft.Extensions.Logging;
using Videos.Application.Interfaces;
using Videos.Application.Services;
using Videos.Domain;

namespace Videos.Application.Jobs
{
    public class Video720ConvertJob : BaseVideoConvertJob
    {
        public Video720ConvertJob(ILogger<BaseVideoConvertJob> logger, IVideoDbContext db) : base(logger, db)
        {
        }

        public override string Quality => "720";
        public override string Prefix => "_en720p";

        protected override async Task<ConversionOptions> GetConversionOptions(string path)
        {
            var metaData = await VideoHelper.GetMetaData(path);
            var width = VideoHelper.GetWidthSize(metaData.VideoData.FrameSize);
            var height = VideoHelper.GetHeightSize(metaData.VideoData.FrameSize);

            var options = new ConversionOptions();
            options.VideoCodec = FFmpeg.NET.Enums.VideoCodec.libvpx__vp9;
            options.VideoFormat = FFmpeg.NET.Enums.VideoFormat.mp4;
            var rotation = VideoHelper.GetRotation(width, height);

            if (rotation == Rotation.Horizontal)
            {
                options.VideoSize = FFmpeg.NET.Enums.VideoSize.Custom;
                options.CustomWidth = -1;
                options.CustomHeight = 720;
            }
            else
            {
                options.VideoSize = FFmpeg.NET.Enums.VideoSize.Custom;
                options.CustomWidth = 720;
                options.CustomHeight = -1;
            }
            return options;
        }
    }
}
