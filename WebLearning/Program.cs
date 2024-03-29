namespace WebLearning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location)!)
                        .ConfigureAppConfiguration((ctx, conf) => {
                            conf.AddJsonFile("learningApiAppSettings.json", optional: true, reloadOnChange: true);
                        })
                        .UseStartup<Startup>();
                });
    }
}