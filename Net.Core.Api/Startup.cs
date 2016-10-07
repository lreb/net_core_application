using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Text;
using Net.Core.Api.Options;
using Microsoft.IdentityModel.Tokens;
using Net.Core.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Net.Core.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            // By default, the configuration doesn't have anything in it. However, it can parse JSON files
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            // adds any environmental values to the configuration
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        /// It represents the root of the configuration file that was formerly represented by the web.config file.
        /// It represents all of the configuration necessary to run the app.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        //secret key
        private const string SecretKey = "needtogetthisfromenvironment";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        // This method gets called by the runtime. Use this method to add services to the container
        /// <summary>
        /// ConfigureServices exists for the explicit reason of adding services to ASP.NET. ASP.NET Core supports Dependency Injection natively, 
        /// and as such this method is adding services to the DI container
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddOptions();
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            var connection = @"Server=(localdb)\mssqllocaldb;Database=ApiDataBase;Trusted_Connection=True;";
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

            // Make authentication compulsory across the board (i.e. shut
            // down EVERYTHING unless explicitly opened up).
            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters(); ;
            // Use policy auth.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DisneyUser", policy => policy.RequireClaim("DisneyCharacter", "IAmMickey"));
            });

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256); //HmacSha256
            });

            // include XML comments to swagger
            var xmlPath = GetXmlCommentsPath();
            // add swagger service to middleware
            services.AddSwaggerGen();
            // swagger description
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "LREB API",
                    Description = "ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact() { Name = "Talking Dotnet", Email = "respinozabarboza@gmail.com.com", Url = "www.facware.com" }
                });
                // include XML comments to swagger
                //options.IncludeXmlComments(xmlPath);
                // added to code to display enum values to swagger
                //options.DescribeAllEnumsAsStrings();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        /// <summary>
        /// You use this method to tell ASP.NET what frameworks you would like to use for this app. This allows you full, detailed control over the HTTP pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                // Validate the token expiry
                //RequireExpirationTime = true,
                ValidateLifetime = true,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
            
            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,
                
            });

            // use swagger framework
            app.UseSwagger();
            app.UseSwaggerUi();

            app.UseMvc();
        }

        /// <summary>
        /// This code to get XML path will work in your local environment as well as in production environment.
        /// </summary>
        /// <returns></returns>
        private string GetXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            return System.IO.Path.Combine(app.ApplicationBasePath, "WebAPIWithSwagger.xml");
        }
    }
}
