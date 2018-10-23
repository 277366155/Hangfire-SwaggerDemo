using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Solitude.Exchange.Core;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace Solitude.Exchange.Hangfire
{
    public class Startup
    {
        /// <summary>
        /// Redis 服务
        /// </summary>
        public static ConnectionMultiplexer Redis;
        public Startup()
        {
            Redis = ConnectionMultiplexer.Connect(BaseCore.Configuration.GetConnectionString("HangfireRedis"));
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //注册hangfire服务
            services.AddHangfire(config => config.UseRedisStorage(Redis));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //注册swagger服务
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1",new Info() {  Title="hangfire接口文档"});

                //添加nuget包：Microsoft.Extensions.PlatformAbstractions
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Solitude.Exchange.Hangfire.xml");
                s.IncludeXmlComments(xmlPath,true);
                s.IncludeXmlComments(Path.Combine(basePath, "Solitude.Exchange.Model.xml"),true);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //启动hangfire服务
            app.UseHangfireServer();

            //启动hangfire面板
            app.UseHangfireDashboard("/hangfire", Options);

            app.UseMvc();
           //启动swagger，以及ui界面
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "");
            });
        }

        public DashboardOptions Options
        {
            get
            {
                return new DashboardOptions
                {
                    AppPath = "",

                    Authorization = new[]  {
                    //引用类库：using Hangfire.Dashboard.BasicAuthorization;
                    new BasicAuthAuthorizationFilter ( new BasicAuthAuthorizationFilterOptions
                    {
                        SslRedirect = false,
                        RequireSsl = false,
                        LoginCaseSensitive = true,
                        Users = new[]
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login ="hangfire",
                                PasswordClear ="123456"
                            }
                        }
                    } )
                }
                };
            }
        }
    }

}
