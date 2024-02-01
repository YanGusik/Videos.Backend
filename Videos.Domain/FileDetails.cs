using System.Text.Json.Serialization;

namespace Videos.Domain
{
    public class FileDetails
    {
        public string? Name { get; set; }
        public string? ThumbnailPath { get; set; }
        public string? Path { get; set; }
        public long? Size { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VideoType Type { get; set; } = VideoType.Video;
        public VideoStreams VideoStreams { get; set; } = new VideoStreams();
        public double? DurationMs { get; set; }

        public Dictionary<string, EncodedFileDetails> EncodedFiles { get; set; } = new Dictionary<string, EncodedFileDetails>();
    }
}
