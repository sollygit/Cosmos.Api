using Cosmos.Api.Configurations;
using Cosmos.Api.HubConfig;
using Cosmos.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Cosmos.Api
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
            // Configurations
            services.Configure<StorageConfig>(Configuration.GetSection("StorageConfig"));

            services.AddCosmosRepository(options => {
                options.CosmosConnectionString = Configuration["RepositoryOptions:CosmosConnectionString"];
            });
            services.AddSingleton<ICandidateService, CandidateService>();
            services.AddSingleton<ICloudTableService, CloudTableService>();
            services.AddSingleton<IStorageService, BlobStorageService>();
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(Configuration["CorsUrl"]));
            });

            services.AddSignalR()
                .AddAzureSignalR();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "CosmosDB API",
                    Description = "A CosmosDB Web API",
                    Contact = new OpenApiContact {
                        Name = @"GitHub Repository",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/sollygit/Cosmos.Api")
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "Blob Storage API" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CosmosDB API");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Blob Storage API");
                c.DocumentTitle = "CosmosDB APIs";
                c.DefaultModelsExpandDepth(0);
                c.RoutePrefix = "";
            });

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            
            app.UseRouting();
            app.UseFileServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CandidateHub>("/candidate");
                endpoints.MapHub<ChartHub>("/chart");
                endpoints.MapHub<BoardHub>("/board");
            });
        }
    }
}
