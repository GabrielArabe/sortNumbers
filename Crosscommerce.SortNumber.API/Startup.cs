using Crosscommerce.SortNumber.Business;
using Crosscommerce.SortNumber.Contract;
using Crosscommerce.SortNumber.Core;
using Crosscommerce.SortNumber.Core.Cache;
using Crosscommerce.SortNumber.External;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.API
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
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Crosscommerce.SortNumber.API", Version = "v1" });
            });

            services.AddSingleton<Serilog.ILogger>((s) =>
            {
                var log = new LoggerConfiguration().WriteTo.File(new JsonFormatter(renderMessage: true), $"log_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.json").CreateLogger();
                return log;
            });

            services.AddScoped<ISortNumbersBusiness, SortNumbersBusiness>();               
            services.AddSingleton<ISorter, Sorter>();
            services.AddTransient<IDienekesApiClient, DienekesApiClient>();
            services.AddSingleton<IApiClient, ApiClient>();
            services.AddSingleton<ICacher, Cacher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crosscommerce.SortNumber.API v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
