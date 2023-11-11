using Rsoi.Lab3.RatingService.Database;
using Rsoi.Lab3.RatingService.HttpApi.Extensions;

namespace Rsoi.Lab3.RatingService.HttpApi;

public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDatabase<RatingContext>()
                .Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e); // Serilog
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults((webBuilder) =>
            {
                webBuilder.UseStartup<Startup>();
            });
}