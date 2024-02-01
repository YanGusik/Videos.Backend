using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Videos.Domain
{
    public class ProcessingDetails
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VideoStatus Status { get; set; } = VideoStatus.Uploaded;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VideoStage Stage { get; set; } = VideoStage.Uploading;
        //public string? Status { get; set; }
        public double? PartsTotal { get; set; }
        public double? PartsProcessed { get; set; }
        public double? TimeLeftMs { get; set; }
    }
}
