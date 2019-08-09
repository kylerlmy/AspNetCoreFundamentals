using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace RedisExample
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
            //services.AddDistributedRedisCache();//老版本中使用，Microsoft.Extensions.Caching.Redis;


            services.AddStackExchangeRedisCache(options =>
            {
                //options.InstanceName = Configuration.GetValue<string>("redis:name");
                //options.Configuration = Configuration.GetValue<string>("redis:host");
                options.Configuration = "192.168.1.110:6379,password=netkyle";
                options.InstanceName = "redis-master";
            });
            services.AddSession();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime liftTime,
            IDistributedCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            liftTime.ApplicationStarted.Register(() =>
            {
                var currentTime = DateTime.UtcNow.ToString();
                var encodeCurrentTimeUc = Encoding.UTF8.GetBytes(currentTime);
                var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(120));
                cache.Set("cahcedTimeUTC", encodeCurrentTimeUc, options);
                //HGET my_hash_key my_hash_field 使用命令行，获取数据类型为哈希表的值
            });

            app.UseSession();

            app.UseMvc();
        }
    }
}
