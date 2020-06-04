using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrBAE.WdmServer.WebApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DrBAE.WdmServer.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Universe.Web.Data;

namespace DrBAE.WdmServer.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppContext.BaseDirectory;
            var host = CreateHostBuilder(args).Build();
            initSeed(host).Wait();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())

                //--- Logging ฐüทร ----
                //.ConfigureLogging(builder => builder.ClearProviders().AddConsole())
                .ConfigureLogging(builder => builder.ClearProviders().AddProvider(new Provider("WebApp")));
        }

        static async Task initSeed(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var logger = sp.GetService<ILogger<Program>>();
            var config = sp.GetService<IConfiguration>();
            try
            {
                using var context = sp.GetService<AppDbContext>();
                if (config.GetValue<bool>("DropDynamicDatabase")) context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                using var dc = sp.GetService<DynamicDbContext>();
                if (config.GetValue<bool>("DropDynamicDatabase")) dc.Database.EnsureDeleted();
                dc.Database.EnsureCreated();

                await AppSeed.AddAdmin(sp, dc);
                await AppSeed.AddConfig(sp, dc);
                await AppSeed.AddLot(sp, dc);
                await AppSeed.AddPigtailReportFormat(sp, dc);
                await AppSeed.AddRawData(sp, dc);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Main(): An error occurred seeding the DB.");
                throw ex;
            }
        }
    }
}
