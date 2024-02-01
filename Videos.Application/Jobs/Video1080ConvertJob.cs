using FFmpeg.NET;
using Microsoft.Extensions.Logging;
using Videos.Application.Interfaces;
using Videos.Application.Services;
using Videos.Domain;

namespace Videos.Application.Jobs
{
    public class Video1080ConvertJob : BaseVideoConvertJob
    {
        public Video1080ConvertJob(ILogger<BaseVideoConvertJob> logger, IVideoDbContext db) : base(logger, db)
        {
        }

        public override string Quality => "1080";
        public override string Prefix => "_en1080p";

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
                options.CustomHeight = 1080;
            }
            else
            {
                options.VideoSize = FFmpeg.NET.Enums.VideoSize.Custom;
                options.CustomWidth = 1080;
                options.CustomHeight = -1;
            }
            return options;
        }
    }
}
