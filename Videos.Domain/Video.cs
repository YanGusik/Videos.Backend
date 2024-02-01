using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Domain
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }

        public FileDetails FileDetails { get; set; } = new FileDetails();
        public ProcessingDetails ProcessingDetails { get; set; } = new ProcessingDetails();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void Processed()
        {
            ProcessingDetails.Status = VideoStatus.Processed;
            ProcessingDetails.TimeLeftMs = 0;
            ProcessingDetails.PartsProcessed = 0;
            ProcessingDetails.PartsTotal = 0;
        }

        public void Error()
        {
            ProcessingDetails.Status = VideoStatus.Failed;
            ProcessingDetails.TimeLeftMs = 0;
            ProcessingDetails.PartsProcessed = 0;
            ProcessingDetails.PartsTotal = 0;
        }
    }
}
