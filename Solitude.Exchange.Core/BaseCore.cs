using Microsoft.Extensions.Configuration;
using System.IO;

namespace Solitude.Exchange.Core
{
    public static class BaseCore
    {
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
    }
}
