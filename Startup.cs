    using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PMAuth.Middleware;

using PMAuth.AuthDbContext;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
    using PMAuth.Models.OAuthGoogle;
    using PMAuth.Services.Abstract;
    using PMAuth.Services.GoogleOAuth;
    using PMAuth.Services.OAuthUniversal;


    namespace PMAuth
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup constructor
        /// </summary>
        /// <param name="configuration">IConfiguration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMAuth", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            });

            services.AddDbContext<BackOfficeContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("BackOfficeContext"))
                , ServiceLifetime.Transient);

            services.AddHttpClient();

            services.AddTransient<IUserProfileReceivingServiceContext, UserProfileReceivingServiceContext>();
            //services.AddTransient<IAccessTokenReceivingService<GoogleTokensModel>, GoogleAccessTokenReceivingService>();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PMAuth v1"));
            }
            app.UseMiddleware<LogMiddleware>();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

            using var scope = app.ApplicationServices.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<BackOfficeContext>();
            await context.Database.MigrateAsync();
        }
    }
}
