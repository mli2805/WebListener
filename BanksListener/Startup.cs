using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UtilsLib;

namespace BanksListener
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("Cors", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetIsOriginAllowed(hostName => true);
            }));

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddSignalR();

            var iniFile = new IniFile();
            iniFile.AssignFile("baliWebApi.ini");
            services.AddSingleton(iniFile);

            var logFile = new LogFile(iniFile);
            logFile.AssignFile("baliWebApi.log");
            services.AddSingleton<IMyLog>(logFile);
            logFile.AppendLine("BankListener WebApi service started");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=KomBanks}/{action=GetLastRate}/{bankId?}");
                endpoints.MapHub<SignalRHub>("/balisSignalRHub");
            });
        }
    }
}
