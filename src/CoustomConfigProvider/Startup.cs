using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoustomConfigProvider.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoustomConfigProvider
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public Startup(IHostingEnvironment env)
        {
            #region sample1

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
            if (appAssembly != null) { builder.AddUserSecrets(appAssembly, optional: true); }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            #endregion sample1
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region sample1
            services.AddSingleton<IConfiguration>(Configuration);
            #endregion sample1

            //获取配置
            var value = Configuration["quote1"];

            //GetValue
            var getConfigValue = Configuration.GetSection("sectionCustom1");
            var getConfigValueUnder = Configuration.GetSection("sectionCustom2:key0");
            var children = getConfigValue.GetChildren();

            var isSectionExsit = getConfigValue.Exists();

            //Bind
            var starship = new Starship();
            Configuration.GetSection("starship").Bind(starship);
            var bindValue = starship.Name;


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var value = Configuration["quote1"];


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
