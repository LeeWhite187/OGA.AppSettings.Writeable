using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OGA.AppSettings.Writeable.JSONConfig
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section) where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<OGA.AppSettings.Writeable.JSONConfig.IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new OGA.AppSettings.Writeable.JSONConfig.cWritableOptions<T>(options, configuration, section.Key);
            });
        }
    }
}

// Below is the original version of this extension, that included an environment variable and a file path string. These two values are not needed in the current implementation.
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Microsoft.Extensions.DependencyInjection
//{
//    public static class ServiceCollectionExtensions
//    {
//        public static void ConfigureWritable<T>(
//            this IServiceCollection services,
//            IConfigurationSection section,
//            string file = "config.json") where T : class, new()
//        {
//            services.Configure<T>(section);
//            services.AddTransient<OGA.AppSettings.Writeable.IWritableOptions<T>>(provider =>
//            {
//                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
//                var environment = provider.GetService<IWebHostEnvironment>();
//                var options = provider.GetService<IOptionsMonitor<T>>();
//                return new OGA.AppSettings.cWritableOptions<T>(environment, options, configuration, section.Key, file);
//            });
//        }
//    }
//}

