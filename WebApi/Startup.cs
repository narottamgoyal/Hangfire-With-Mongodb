using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HangfireWithMongoDb.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDbClient;
using System;

namespace HangfireWithMongoDb
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HangfireWithMongoDb", Version = "v1" });
            });

            RegisterDbDependancies(services);

            #region Hangfire

            var options = new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new DropMongoMigrationStrategy(),
                    BackupStrategy = new NoneMongoBackupStrategy()
                }
            };

            services.AddHangfire(x => x.UseMongoStorage(@"mongodb://localhost:27017/Hangfire_Master_db", options));
            services.AddHangfireServer();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HangfireWithMongoDb v1"));
            }

            #region Hangfire

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            // Hangfire first job
            BackgroundJob.Enqueue(() => Console.WriteLine("Wellcome to hangfire with mongodb!"));

            #endregion

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void RegisterDbDependancies(IServiceCollection services)
        {
            services.AddSingleton<IDatabaseContext>(new DatabaseContext(
                @"mongodb://localhost:27017/",
                "Application_db"));

            services.AddSingleton<IHangFireOpRepository, HangFireOpRepository>();
        }
    }
}
