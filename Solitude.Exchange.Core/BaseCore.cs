using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Solitude.Exchange.Core
{
    public static class BaseCore
    {
        #region 配置文件读取
        private static IConfigurationRoot _configuration;
        private static object lockObj=new object();
        /// <summary>
        /// 读取配置
        /// </summary>
        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    lock (lockObj)
                    {
                        if (_configuration == null)
                        {
                            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                            _configuration = builder.Build();
                        }
                    }
                }

                return _configuration;
            }
        }

        #endregion 配置文件读取

        #region HttpContext
        public static IServiceProvider ServiceProvider { get; set; }

        private static IHttpContextAccessor _currentAccessor;

        public static IHttpContextAccessor CurrentAccessor
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
                return (IHttpContextAccessor)factory;
            }
        }

        /// <summary>
        /// 当前http请求上下文
        /// </summary>
        public static HttpContext CurrentContext
        {
            get
            {
                return CurrentAccessor.HttpContext;
            }
        }
        #endregion HttpContext
    }
}
