using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PlayCat.DataServices;

namespace PlayCat.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
    
    public class BloggingContextFactory : IDesignTimeDbContextFactory<PlayCatDbContext>
    {
        public PlayCatDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PlayCatDbContext>();
            optionsBuilder.UseNpgsql("User ID=postgres;Password=1Q2w3e4r;Host=localhost;Database=PlayCat;");

            return new PlayCatDbContext(optionsBuilder.Options);
        }
    }
}
