using System.IO;
using EmergenceGuardian.FFmpeg;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PlayCat.DataServices;
using PlayCat.Music;
using Swashbuckle.AspNetCore.Swagger;
using ServiceProvider = PlayCat.DataServices.ServiceProvider;

namespace PlayCat.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration      = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration      Configuration      { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();
            services.AddOptions();
            services.Configure<FolderOptions>(x =>
            {
                var section = Configuration.GetSection("FolderPaths");
                x.AudioFolderPath = HostingEnvironment.ContentRootPath + section.GetValue<string>("AudioFolderPath");
                x.VideoFolderPath = HostingEnvironment.ContentRootPath + section.GetValue<string>("VideoFolderPath");
            });
            services.Configure<AudioOptions>(Configuration.GetSection("AudioInfo"));
            services.Configure<VideoRestrictsOptions>(Configuration.GetSection("VideoRestricts"));

            services.AddDbContext<PlayCatDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            ServiceProvider.RegisterServices(services);

            //Windows
            FFmpegConfig.FFmpegPath = @"E:\Downloads\ffmpeg-20180720-3870ed7-win64-static\bin\ffmpeg.exe";
            
            //Mac OS

            //FFmpegConfig.FFmpegPath = @"/Users/admin/Documents/ffmpeg";
            
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"}); });
        }

        private void ServeFromDirectory(IApplicationBuilder app, IHostingEnvironment env, string path)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, path)
                ),
                RequestPath = "/" + path
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, PlayCatDbContext dbContext)
        {
            app.UseCors(builder => builder.AllowAnyMethod()
                                          .AllowAnyHeader()
                                          .AllowAnyOrigin());
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
           

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


            app.UseDeveloperExceptionPage();


            ServeFromDirectory(app, env, "Audio");


            app.UseMvc();
        }
    }
}