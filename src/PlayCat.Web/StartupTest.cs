using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlayCat.DataService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;
using EmergenceGuardian.FFmpeg;
using Microsoft.AspNetCore.Mvc;
using PlayCat.Music;

namespace PlayCat
{
    public class StartupTest
    {
        public StartupTest(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
            Configuration = configuration;            
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddOptions();
            services.Configure<FolderOptions>(x =>
            {
                IConfigurationSection section = Configuration.GetSection("FolderPaths");
                x.AudioFolderPath = HostingEnvironment.ContentRootPath + section.GetValue<string>("AudioFolderPath");
                x.VideoFolderPath = HostingEnvironment.ContentRootPath + section.GetValue<string>("VideoFolderPath");
            });
            services.Configure<AudioOptions>(Configuration.GetSection("AudioInfo"));
            services.Configure<VideoRestrictsOptions>(Configuration.GetSection("VideoRestricts"));

            var connectionString = Configuration.GetConnectionString("DefaultConnection"); 
            services.AddDbContext<PlayCatDbContext>(options => options.UseNpgsql(connectionString));

            DataService.ServiceProvider.RegisterServices(services);
            
            FFmpegConfig.FFmpegPath = @"E:\Downloads\ffmpeg-20180720-3870ed7-win64-static\bin\ffmpeg.exe";
        }

        private void ServeFromDirectory(IApplicationBuilder app, IHostingEnvironment env, string path)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, path)),
                RequestPath = "/" + path
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            ServeFromDirectory(app, env, "node_modules");
            //ServeFromDirectory(app, env, "app");
            ServeFromDirectory(app, env, "Audio");

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 &&
                   !Path.HasExtension(context.Request.Path.Value) &&
                   !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
