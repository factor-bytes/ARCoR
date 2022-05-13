using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ARCoR.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;


namespace ARCoR.WebApi
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        string sqlConnectionString = string.Empty;
        private static string _applicationPath = string.Empty;
        public Startup(IWebHostEnvironment env)
        {
            _applicationPath = env.WebRootPath;
            // Setup configuration sources.

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            //try
            //{
            //    useInMemoryProvider = bool.Parse(Configuration["AppSettings:InMemoryProvider"]);
            //}
            //catch { }

            services.AddDbContext<DatabaseContext>(options =>
                options
                        .UseNpgsql(Configuration.GetConnectionString("VehicleQuotesContext"))
                        .UseSnakeCaseNamingConvention()
                        .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                        .EnableSensitiveDataLogging() //Disable this in production as this ca expose sensitive inforamtion
            );


            // Repositories
            //services.AddScoped<IScheduleRepository, ScheduleRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IAttendeeRepository, AttendeeRepository>();

            // Automapper Configuration
            //AutoMapperConfiguration.Configure();

            // Enable Cors
            services.AddCors();

            // Add MVC services to the services container.
            services.AddMvc()
                .AddJsonOptions(opts =>
                {
                    // Force Camel Case to JSON
                    //opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddDatabaseDeveloperPageExceptionFilter();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VehicleQuotes v1");
                    c.RoutePrefix = "";
                });
            }

        }
    }
}
