using System;
using System.Collections.Generic;
using System.IO;
using HttpClient.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HttpClient.MobileAppService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => { options.AllowSynchronousIO = true; })
                .ConfigureAppConfiguration((hostingContext, config) => {
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddUserSecrets<Startup>();
                        config.AddInMemoryCollection(
                            new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("GitHub:ApiBaseUrl", GitHubConstants.ApiBaseUrl)
                                });
                        config.AddCommandLine(args);
                    })
                .UseStartup<Startup>();
        }

    }
}