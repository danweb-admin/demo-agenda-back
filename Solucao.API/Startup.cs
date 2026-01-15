using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Solucao.API.Configurations;
using Solucao.API.Services;
using Solucao.Application.Data;
using Solucao.Application.Utils;

namespace Solucao.API
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
            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", builder =>
                {
                    builder
                        .WithOrigins("https://www.sjc-laser-agenda.site")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                        // NÃO usar AllowCredentials com JWT no header
                });
            });


            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddControllers().AddNewtonsoftJson(options =>
                  options.SerializerSettings.ReferenceLoopHandling =
                  Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Solucao.API", Version = "v1" });
            });

            // .NET Native DI Abstraction
            services.AddDependencyInjectionConfiguration();

            // Auto Mapper
            services.AddAutoMapperConfiguration();

            var configurationKey = Environment.GetEnvironmentVariable("KeyMD5");
            var key = Encoding.ASCII.GetBytes(configurationKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddHttpClient();

            services.AddHttpContextAccessor();

            var server = Environment.GetEnvironmentVariable("DbServer");
            var port = Environment.GetEnvironmentVariable("DbPort");
            var user = Environment.GetEnvironmentVariable("DbUser");
            var password = Environment.GetEnvironmentVariable("Password");
            var database = Environment.GetEnvironmentVariable("Database");
            var development = Environment.GetEnvironmentVariable("Development");
            string connectionString = string.Empty;

            if (development == "True")
                connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SolucaoDB;";
            else
                connectionString = $"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}";

            services.AddDbContext<SolucaoContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger)
        {
            DatabaseManagementService.MigrationInitialisation(app);

            
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Solucao.API v1"));
            

            /*INICIO DA CONFIGURAÇÃO - PROMETHEUS*/
            // Custom Metrics to count requests for each endpoint and the method
            var counter = Metrics.CreateCounter("apimetric", "Counts requests to the ApiMetrics API endpoints",
                new CounterConfiguration
                {
                    LabelNames = new[] { "method", "endpoint" }
                });

            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });

            // Use the prometheus middleware
            app.UseMetricServer();
            app.UseHttpMetrics();

            /*FIM DA CONFIGURAÇÃO - PROMETHEUS*/

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            app.UseHttpsRedirection();

            app.UseRouting();

            // ⬇️ CORS TEM QUE VIR AQUI
            app.UseCors("FrontendPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    logger.LogWarning($"Unauthorized request - {context.Request.Method} - {context.Request.Path}");
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
