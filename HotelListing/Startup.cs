using HotelListing.Config;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using HotelListing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing
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

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));

            /////////////////////////////////////
            // CACHING
            /////////////////////////////////////
            //services.AddResponseCaching();
            services.ConfigureHttpCacheHeaders();
            /////////////////////////////////////


            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureJwt(this.Configuration);

            // CORS
            services.AddCors(cors =>
            {
                cors.AddPolicy("CorsPolicyAllowAll", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // AUTOMAPPER
            services.AddAutoMapper(typeof(MapperConfig));

            // UNIT OF WORK
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // AUTH MANAGER
            services.AddScoped<IAuthManager, AuthManager>();

            // SWAGGER
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });

            //services.AddControllers().AddNewtonsoftJson(op => op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // USE FOR GLOBAL CACHING
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            services.AddControllers
            (
                config => { config.CacheProfiles.Add("CacheDuration-120Seconds", new CacheProfile { Duration = 120 }); }
            )
            .AddNewtonsoftJson(op => op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.ConfigureVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));

            app.ConfigureExceptionHandler();
            app.UseHttpsRedirection();

            app.UseCors("CorsPolicyAllowAll");

            app.UseResponseCaching();
            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
