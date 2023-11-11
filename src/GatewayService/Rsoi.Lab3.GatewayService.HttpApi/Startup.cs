using Microsoft.OpenApi.Models;
using Rsoi.Lab3.GatewayService.Services.BackgroundServices;
using Rsoi.Lab3.GatewayService.Services.Clients;
using Rsoi.Lab3.GatewayService.Services.Confiugration;

namespace Rsoi.Lab3.GatewayService.HttpApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddNewtonsoftJson();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenAPI definition", Version = "v1" });
            
        });
        services.AddSwaggerGenNewtonsoftSupport();

        services.AddControllers()
            .AddNewtonsoftJson();

        services.Configure<ServiceConfiguration>(Configuration.GetSection(nameof(ServiceConfiguration)));
        
        services.AddSingleton<LibraryServiceClient>();
        services.AddSingleton<RatingServiceClient>();
        services.AddSingleton<ReservationServiceClient>();
        services.AddSingleton<UndoneRequestsQueue>();
        services.AddHostedService<RequestRetryBackgroundService>();

    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenAPI definition v1"));

        app.UseRouting();
        app.UseCors("policy"); // Must be after Routing and before Authorization middlewares.

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}