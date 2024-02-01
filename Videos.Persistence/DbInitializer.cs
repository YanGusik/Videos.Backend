using Microsoft.EntityFrameworkCore;

namespace Videos.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(VideoDbContext context) 
        {
            var con = context.Database.GetConnectionString();
            context.Database.EnsureCreated();
        }
    }
}
