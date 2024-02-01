using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Videos.Domain;

namespace Videos.Persistence.EntityTypeConfigurations
{
    public class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(250);

            builder.Property(e => e.FileDetails).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<FileDetails>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })).HasColumnType("jsonb");

            builder.Property(e => e.ProcessingDetails).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<ProcessingDetails>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })).HasColumnType("jsonb");

            // stupid ef core dont work with dictionary
            //builder.OwnsOne(video => video.FileDetails, builder =>
            //   {
            //       builder
            //       //.OwnsOne(f => f.EncodedFiles, builder =>
            //       //{
            //       //    builder.OwnsMany(f => f.Values, builder =>{builder.ToJson();}).ToJson(); 
            //       //})
            //       .OwnsOne(f => f.VideoStreams, builder => { builder.ToJson(); })
            //       .ToJson();
            //   })
            //   .OwnsOne(video => video.ProcessingDetails, builder => { builder.ToJson(); });
        }
    }
}
