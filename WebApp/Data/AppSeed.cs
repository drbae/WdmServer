using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrBAE.TnM.Utility;
using DrBAE.TnM.WdmAnalyzer;
using DrBAE.WdmServer.ExceptionProcessing;
using DrBAE.WdmServer.Models;
using DrBAE.WdmServer.WebApp.Controllers;
using DrBAE.WdmServer.WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MT = DrBAE.WdmServer.Models.ConfigType;

namespace DrBAE.WdmServer.WebApp.Data
{
    public class AppSeed
    {
        static IdentityUserRole<string>? _admin;
        public static async Task AddAdmin(IServiceProvider sp, IdentityDbContext context)
        {
            var config = sp.GetService<IConfiguration>();
            var section = config.GetSection("SeedData");
            var username = section.GetValue<string>("Username");
            var email = section.GetValue<string>("Email");
            var password = section.GetValue<string>("Password");

            //using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            //using var context = serviceProvider.GetService<AppDbContext>();
            if (context.Users.Any()) return;   // DB has been seeded

            //사용자추가
            var adminUser = new IdentityUser { UserName = username, Email = email, EmailConfirmed = true };
            await sp.GetService<UserManager<IdentityUser>>().CreateAsync(adminUser, password);

            //롤추가
            var roleManager = sp.GetService<RoleManager<IdentityRole>>();
            var adminRole = new IdentityRole(AppDbContext.AdminRoleName);
            await roleManager.CreateAsync(adminRole);
            await roleManager.CreateAsync(new IdentityRole(AppDbContext.DeleteRoleName));
            await roleManager.CreateAsync(new IdentityRole(AppDbContext.NormalRoleName));

            //롤할당
            _admin = context.UserRoles.Add(new IdentityUserRole<string>() { UserId = adminUser.Id, RoleId = adminRole.Id }).Entity;

            context.SaveChanges();
        }

        public static async Task AddConfig(IServiceProvider sp, DbContext context)
        {
            if (context.Set<ConfigModel>().Any()) return;

            (string fileName, MT type)[] files = new[]
            {
                ("raw_4CH_NP", MT.Raw), ("raw_4CH_POL", MT.Raw),

                (TypeOfDUT.CWDM_RICH_DEMUX_SHIFT.ToString(), MT.Analysis), 
                (TypeOfDUT.LWDM_DEMUX_INTEL.ToString(), MT.Analysis), 
                (TypeOfDUT.CWDM_RICH_DEMUX_POL.ToString(), MT.Analysis),

                ("Report_Pol_N4", MT.Report),
                ("Report_RICH_CH_Sum", MT.Report), ("Report_RICH_DUT_Sum", MT.Report),
                ("Report_RICH_POL_CH_Sum", MT.Report), ("Report_RICH_POL_DUT_Sum", MT.Report),
                ("Report_LWDM_CH_Sum", MT.Report), ("Report_LWDM_DUT_Sum", MT.Report),

                ("PigtailConfig_NOVA", MT.Pigtail),("PigtailConfig_LWDM", MT.Pigtail)
            };

            foreach (var fn in files)
            {
                var fileName = Path.Combine("TestData", $"{fn.fileName}.ini");
                var model = new ConfigModel();
                {
                    model.ConfigType = fn.type;
                    model.Content = fn.type != MT.Analysis ? File.ReadAllText(fileName) : fn.fileName;
                    model.Name = fn.fileName;//Path.GetFileNameWithoutExtension(fileName)
                };
                await ConfigModelController.buildModel(sp, context, model);
            };
        }

        public static async Task AddLot(IServiceProvider sp, DbContext context)
        {
            if (context.Set<LotModel>().Any()) return;

            (string lotName, DateTime lotDate)[] Lots = new[] {
                ("Lot1", DateTime.Now),
                ("Lot2", DateTime.Parse("2019-05-18 18:11:22")),
                ("Lot3", DateTime.Parse("2019-05-18 17:17:27"))
            };

            foreach (var item in Lots)
            {
                var model = new LotModel()
                {
                    LotName = item.lotName,
                    LotDate = item.lotDate
                };
                context.Add(model);
            };
            await context.SaveChangesAsync();
        }

        public static async Task AddPigtailReportFormat(IServiceProvider sp, DbContext context)
        {
            if (context.Set<PigtailReportFormat>().Any()) return;

            (string file, string config)[] formats = new[]
            { ("LanWDM_Report_Format.xlsx", "PigtailConfig_LWDM"), ("Nova_Report_Format.xlsx", "PigtailConfig_NOVA") }
            .Select(x => (Path.Combine("TestData", x.Item1), x.Item2)).ToArray();

            foreach (var format in formats)
            {
                var config = context.Set<ConfigModel>().Where(x => x.Name == format.config).First();
                var model = new PigtailReportFormat()
                {
                    Name = Path.GetFileNameWithoutExtension(format.file),
                    FormFile = File.ReadAllBytes(format.file),
                    ConfigId = config.Id,
                };
                context.Add(model);
            }
            await context.SaveChangesAsync();
        }

        public static async Task AddRawData(IServiceProvider sp, DbContext context)
        {
            if (context.Set<RawUpload>().Any()) return;

            var config = context.Set<ConfigModel>().Where(x => x.Name.Contains("raw_4CH_NP")).First();

            var model = new RawUpload();
            model.ConfigId = config.Id;
            model.UserId = _admin?.UserId ?? throw ExBuilder.Create("Can't find user id").Throw();
            model.Description = $"Seed by DrBAE";

            var dir = Path.Combine("TestData", "Rich");
            var files = Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories).Select(x => (x, File.ReadAllText(x)));
            model.ZipFile = Compression.Zip(files);

            await RawUploadController.buildModel(sp, context, model, config);
        }
    }
}
