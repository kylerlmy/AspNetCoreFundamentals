using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoustomConfigProvider.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoustomConfigProvider
{
    public class Program
    {
        public static Dictionary<string, string> arrayDict = new Dictionary<string, string>
        {
            {"array:entries:0", "value0"},
            {"array:entries:1", "value1"},
            {"array:entries:2", "value2"},
            {"array:entries:4", "value4"},
            {"array:entries:5", "value5"}
        };


        public static void Main(string[] args)
        {
            // CreateWebHostBuilder(args).Build().Run();


            //AddJsonFile is automatically called twice when you initialize a new WebHostBuilder with CreateDefaultBuilder. The method is called to load configuration from:
            //appsettings.json – This file is read first. The environment version of the file can override the values provided by the appsettings.json file.
            //appsettings.{ Environment}.json – The environment version of the file is loaded based on the IHostingEnvironment.EnvironmentName.  
            var webBuilder = WebHost.CreateDefaultBuilder(args);

            //添加配置方法2：Call ConfigureAppConfiguration when building the host to specify the app's configuration  

            //This sequence of providers can be created for the app (not the host) with a ConfigurationBuilder and a call to its Build method in Startup

            webBuilder = webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                          {
                              config.SetBasePath(Directory.GetCurrentDirectory());
                              config.AddInMemoryCollection(arrayDict);
                              config.AddJsonFile("json_array.json", optional: false, reloadOnChange: true);
                              config.AddJsonFile("starship.json", optional: false, reloadOnChange: false);
                              config.AddXmlFile("tvshow.xml", optional: false, reloadOnChange: false);
                              config.AddEFConfiguration(options => options.UseInMemoryDatabase("InMemoryDb"));//Custom configuration provider    
                              config.AddCommandLine(args);
                          });
            webBuilder.UseKestrel().UseStartup<Startup>();

            webBuilder.Build().Run();


            //添加配置方法1:creating a WebHostBuilder directly, call UseConfiguration with the configuration:

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                .Build();
            var webbuiler2 = new WebHostBuilder();

            webbuiler2.UseConfiguration(configBuilder)
                .UseKestrel()
                .UseStartup<Startup>();

            webbuiler2.Build().Run();


        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();




        //  


    }
}
