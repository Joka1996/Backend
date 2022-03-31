using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Litium.Accelerator.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Simplification", "RCS1021:Convert lambda expression body to expression-body.", Justification = "<Pending>")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<Runtime.RequestModelActionFilter>();
            })
            .AddSessionStateTempDataProvider()
            .AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/Views/Blocks/{0}" + RazorViewEngine.ViewExtension);
            })
#if DEBUG
            .AddRazorRuntimeCompilation()
#endif
            .AddNewtonsoftJson();

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = false;
            });

            services.AddLitiumApplication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseRequestTime();
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseLitiumExceptionPage();
            }

            // Litium application handle Https redirections setup with HSTS on the domain name.
            // We dont need to call the app.UseHttpsRedirection.
            // app.UseHttpsRedirection();

            app.UseLitiumPageNotFoundPage();
            app.UseLitiumUrlRedirect();

            app.UseLitiumStaticFiles();
            app.UseDynamicResponseCaching();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseLitiumIdentity();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseLitiumRouting();
            app.UseSession();

            app.UseLitiumCart();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
                options.MapLitiumEndpoints();
            });
        }
    }
}
