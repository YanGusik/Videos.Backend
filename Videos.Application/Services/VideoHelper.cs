using FFmpeg.NET;
using Videos.Domain;

namespace Videos.Application.Services
{
    public class VideoHelper
    {
        public static async Task<FileDetails> GetFileDetails(string path)
        {
            var metaData = await GetMetaData(path);
            if (metaData == null) { throw new Exception("File not found"); }

            var width = GetWidthSize(metaData.VideoData.FrameSize);
            var height = GetHeightSize(metaData.VideoData.FrameSize);

            var videoStreams = new VideoStreams()
            {
                WidthPixels = width,
                HeightPixels = height,
                AspectRatio = GetAspectRation(width, height),
                BitrateBps = metaData.VideoData.BitRateKbs,
                Codec = metaData.VideoData.Format,
                FrameRateFps = metaData.VideoData.Fps,
                Rotation = GetRotation(width, height),
            };

            return new FileDetails()
            {
                Path = path,
                Name = metaData.FileInfo.Name,
                DurationMs = metaData.Duration.TotalMilliseconds,
                Size = metaData.FileInfo.Length,
                Type = VideoType.Video,
                VideoStreams = videoStreams,
                EncodedFiles = new Dictionary<string, EncodedFileDetails>()
            };
        }

        public static async Task<MetaData> GetMetaData(string path)
        {
            var ffmpeg = new Engine("ffmpeg");
            var inputFile = new InputFile(path);
            return await ffmpeg.GetMetaDataAsync(inputFile, new CancellationToken());
        }

        public static int GetWidthSize(string frameSize)
        {
            string resolution = frameSize.Split("x")[0];
            return int.Parse(resolution);
        }

        public static int GetHeightSize(string frameSize)
        {
            string resolution = frameSize.Split("x")[1];
            return int.Parse(resolution);
        }

        public static string GetAspectRation(int width, int height)
        {
            return string.Format("{0}:{1}", width / GCD(width, height), height / GCD(width, height));
        }

        public static Rotation GetRotation(int width, int height)
        {
            return width > height ? Rotation.Horizontal : Rotation.Vertical;
        }

        public static int GCD(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }
    }
}
