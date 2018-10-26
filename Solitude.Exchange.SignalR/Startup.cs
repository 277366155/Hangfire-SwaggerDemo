using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solitude.Exchange.Core;
using System;

namespace Solitude.Exchange.SignalR
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
            //注册SignalR服务
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("SignalrCore",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });
            services.AddSingleton<IServiceProvider, ServiceProvider>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider sp)
        {
            BaseCore.ServiceProvider = sp;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //添加静态文件管理，默认读取wwwroot目录
            app.UseStaticFiles();
            
            //也可以读取指定目录
            //var staticfile = new StaticFileOptions();
            //staticfile.FileProvider = new PhysicalFileProvider(@"C:\");//指定目录 这里指定C盘,也可以是其它目录
            //app.UseStaticFiles(staticfile);

            // 跨域支持
            app.UseCors("SignalrCore");
            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalrHubs>("/signalrHubs");
            });
           // app.UseWebSockets();

            app.UseMvc();
        }
    }

}
