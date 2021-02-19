using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BasicObjects;

namespace WebAPI
{
    public class Startup
    {
        private SingletonDictionary singletonDictionary ;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            singletonDictionary = SingletonDictionary.GetInstance();
            string FileFolderPath = (string) Configuration["FileFolder"];
            singletonDictionary.AddOrUpdate("FileFolder", FileFolderPath);
        }
        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("*");
                    });
            });

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();

            app.Use((context, next) =>
            {
                context.Items["__CorsMiddlewareInvoked"] = true;
                return next();
            });


            singletonDictionary = SingletonDictionary.GetInstance();
            singletonDictionary.AddOrUpdate("env.WebRootPath", env.WebRootPath);
            singletonDictionary.AddOrUpdate("env.ContentRootPath", env.ContentRootPath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();
            app.UseDefaultFiles();

            StaticFileOptions staticFileOptions = new StaticFileOptions();
            staticFileOptions.ServeUnknownFileTypes = true;
            app.UseStaticFiles(staticFileOptions);



            //app.UseFileServer();  //shorthand for app.UseDefaultFiles(); app.UseStaticFiles();
            //app.UseHttpsRedirection(); //only works with a SSL Certificate
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

         //   app.UseSpa(x => {});

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "wwwroot";  //???????????????

             //   if (env.IsDevelopment()){spa.UseAngularCliServer(npmScript: "start");}
            });
            
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

        }
    }
}
