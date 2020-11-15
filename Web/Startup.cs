using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Infrastructure.Repositories;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
using Infrastructure;
/*using Microsoft.AspNetCore.Mvc;
using Web.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using Microsoft.AspNet.OData.Query;*/
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.ApplicationParts;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers;
using AutoMapper;
using Web.Extensions;
using Infrastructure.Interfaces;
using Web.Configurations;
using Microsoft.OData.UriParser;
using Microsoft.AspNet.OData;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;

namespace Web
{
    public class Startup
    {
        public static SecurityKey PrivateKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //var serviceProvider = services.BuildServiceProvider();

            PrivateKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("0d5b3235a8b403c3dab9c3f4f65c07fcalskd234n1k41230")
            );

            //Authentication 
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = PrivateKey,
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetValue<string>("Name"),
                    ValidateAudience = true,
                    ValidAudience = Configuration.GetValue<string>("Client"),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(5)
                };
            });

            //Authorization
            services.AddAuthorization(options =>
            {
                //options.AddPolicy("read:messages", policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", domain)));
            });

            //
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddControllers()
                /*.AddJsonOptions(options => { 
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new SecurityConverter());
                })*/
                .ConfigureApplicationPartManager(options =>
                {
                    options.ApplicationParts.Add(new GenericControllerApplicationPart(
                        new ApiVersion(DateTime.Now)
                    ));
                })
                .AddControllersAsServices();
            // format the version as "'v'major[.minor][-status]"
            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddOData().EnableApiVersioning();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                //options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddSingleton<ILoggerFactory>(LoggerFactory.Create(builder => builder
                /*.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)*/
                .AddConsole()));

            services.AddDbContext<NorthwindContext>(options => options
                //.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    options => options.EnableRetryOnFailure()));
            services.AddTransient<ISession, EfSession>();
            services.AddTransient(typeof(IRepository<,>), typeof(EfRepository<,>));
            //services.AddTransient(typeof(IRepository<>), typeof(EfRepository<,>));
            /*public delegate IService ServiceResolver(string key);
            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "A":
                        return serviceProvider.GetService<ServiceA>();              
                    default:
                        throw new KeyNotFoundException();
                }
            });
            ctor(ServiceResolver sa) { _sa = sa("A"); }*/
            /*services.AddTransient<ODataValidationSettings>(f => { 
                var vs = new ODataValidationSettings 
                {
                    AllowedArithmeticOperators = AllowedArithmeticOperators.All,
                    AllowedFunctions = AllowedFunctions.All,
                    AllowedLogicalOperators = AllowedLogicalOperators.All,
                    //AllowedOrderByProperties = new Collection<string>("None,".Split(",".ToCharArray())),
                    AllowedQueryOptions = AllowedQueryOptions.All,// | AllowedQueryOptions.Format,
                    MaxOrderByNodeCount = 2,
                    MaxAnyAllExpressionDepth = 2,
                    MaxNodeCount = 10,
                    MaxSkip = int.MaxValue,
                    MaxTop = 100,
                    MaxExpansionDepth = 2
                };
                //vs.AllowedOrderByProperties.Add("LastName");
                return vs;
            });

            services.AddTransient<ODataQuerySettings>(f => new ODataQuerySettings 
            {                
            });*/

            /*services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<NorthwindContext>();*/

            services.AddCors(config =>
            {
                config.AddDefaultPolicy(builder => builder
                    .AllowAnyOrigin()
                    //.WithOrigins("http://localhost:5000", "https://localhost:5001", "http://localhost:5004")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
                config.AddPolicy("AllowSpecificOrigins", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            /*services.AddODataApiExplorer(options =>
            {
                var queryOptions = options.QueryOptions;
                //options.GroupNameFormat = "'v'VVV";
                //options.SubstituteApiVersionInUrl = true;
                queryOptions.Controller<GenericController<Products, int>>()
                    .Action(m => m.Get(default))
                    .AllowOrderBy("productId, productName".Split(','))
                    .Allow(Top | Count)
                    .AllowTop(max: 2)
                    .AllowSkip(max: 3);
            });*/

            //services.Configure<ODataValidationSettings>(Configuration.GetSection("OData"));
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, VersionedODataModelBuilder modelBuilder, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Authentication middleware to the middleware pipeline
            app.UseAuthentication();

            //app.UseHttpsRedirection();
            //app.UseCors(AllowOrigins);

            app.UseCors(/*config => config
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()*/);

            app.UseRouting();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                options.RoutePrefix = string.Empty;
                options.OAuthClientId("demo_api_swagger");
                options.OAuthAppName("Demo API - Swagger"); // presentation purposes only           
            });

            app.UseMvc(config =>
            {
                config.EnableDependencyInjection(/*m => m.AddService(
                    Microsoft.OData.ServiceLifetime.Singleton,
                    typeof(ODataUriResolver),
                    typeof(UnqualifiedCallAndEnumPrefixFreeResolver))*/);
                //config.SetDefaultODataOptions()
                //config.SetDefaultQuerySettings()
                //config.Count().Filter().OrderBy().Expand().Select().MaxTop(int.MaxValue);
                config.MapVersionedODataRoutes("odata", "v{version:apiVersion}", modelBuilder.GetEdmModels());
                DynamicConfiguration.AddOrderRoute(config);
            });
            //DynamicConfiguration.AddOrderRoute();
        }
    }
}

