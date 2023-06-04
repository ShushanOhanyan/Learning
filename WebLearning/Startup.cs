namespace WebLearning;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method is used to configure services that the application will use.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        // Add other services here as needed
    }

    // This method is used to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Customize error handling for production environment
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Serve static files (css, js, images, etc.)
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            // Add other endpoints as needed
        });
    }
    
}