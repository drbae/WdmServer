using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using DrBAE.WdmServer.WebApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using DrBAE.TnM.Utility;
using System.IO;
using Ko.Pigtail;
using Microsoft.AspNetCore.Identity.UI.Services;
using DrBAE.WdmServer.WebUtility;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Universe.Web.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DrBAE.WdmServer.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
            _section = configuration.GetSection("LoaderData");
        }
        readonly IConfiguration _config;
        readonly IWebHostEnvironment _env;
        readonly IConfigurationSection _section;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection sc)
        {
            configHttp(sc);
            configDb(sc);

            configIdentity(sc);//사용자 인증 및 관리자 계정, Email sender
            sc.AddControllersWithViews().AddRazorRuntimeCompilation();
            sc.AddRazorPages();

            //사이트전체 인가받은 사람만 사용가능함
            sc.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            //TODO: 아래 문장이 필요한지 검토
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddNewtonsoftJson();


            configBlazor(sc);
            configLogic(sc);
        }

        void configHttp(IServiceCollection sc)
        {
            sc.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            sc.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.KeyLengthLimit = int.MaxValue;

                options.MultipartHeadersCountLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
            });
            sc.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));
        }

        void configDb(IServiceCollection sc)
        {
            var files = _config.GetSection("ModelAssemblyFile").Get<string[]>();
            var ob = new DbContextOptionsBuilder().UseNpgsql(_config.GetConnectionString("DynamicConnection")).EnableSensitiveDataLogging(true);
            sc.AddScoped(s => DynamicDbContext.Create(ob, files));

            sc.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_config.GetConnectionString(_config.GetConnectionString("CurrentName")));
                options.EnableSensitiveDataLogging(true);
            });
        }

        void configIdentity(IServiceCollection sc)
        {
            sc.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddDefaultUI()
                .AddEntityFrameworkStores<DynamicDbContext>();

            sc.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
                //options.User.AllowedUserNameCharacters = "whatever";
                options.User.AllowedUserNameCharacters = null;
                options.User.RequireUniqueEmail = false;
            });
            sc.AddScoped<AppSeed>();

            sc.AddTransient<IEmailSender, EmailSender>();
            sc.Configure<EmailOptions>(_config.GetSection("EmailSettings"));
        }

        void configBlazor(IServiceCollection sc)
        {
            sc.AddServerSideBlazor();
            sc.AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));
        }

        void configLogic(IServiceCollection sc)
        {
            sc.AddSingleton<IPigtailLogic>(sp => new PigtailLogic());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            configAppBlazor(app);
        }

        void configAppBlazor(IApplicationBuilder app)
        {
            if (_env.IsDevelopment()) app.UseBlazorDebugging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                //endpoints.MapFallbackToPage("/_Host"); //server Blazor
                //endpoints.MapFallbackToClientSideBlazor<HostedBlazor.Client.Startup>("index.html"); //client Blazor
            });

            //app.UseClientSideBlazorFiles<HostedBlazor.Client.Startup>(); //client Blazor
        }

    }//class

}
