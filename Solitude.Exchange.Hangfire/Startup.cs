using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Solitude.Exchange.Core;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;

namespace Solitude.Exchange.Hangfire
{
    public class Startup
    {
        /// <summary>
        /// Redis 服务
        /// </summary>
        public static  ConnectionMultiplexer Redis;
        public Startup()
        {
            Redis = ConnectionMultiplexer.Connect(BaseCore.Configuration.GetConnectionString("HangfireRedis"));
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //注册hangfire服务
            services.AddHangfire(config => config.UseRedisStorage(Redis));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //注册swagger服务
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1",new Info() {  Title="hangfire接口文档"});

                //添加nuget包：Microsoft.Extensions.PlatformAbstractions
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                s.IncludeXmlComments(Path.Combine(basePath, "Solitude.Exchange.Hangfire.xml"), true);
                s.IncludeXmlComments(Path.Combine(basePath, "Solitude.Exchange.Model.xml"),true);

                //添加header验证信息
                //c.OperationFilter<SwaggerHeader>();
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };
                s.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                s.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
            BaseCore.ServiceProvider = svp;
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
