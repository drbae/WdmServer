using System;
using System.Collections.Generic;
using System.Text;
using DrBAE.WdmServer.WebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DrBAE.WdmServer.Models;

namespace DrBAE.WdmServer.WebApp.Data
{
#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
    public class AppDbContext : IdentityDbContext
    {
        #region ---- 인증에 필요한 RoleName ----
        public const string AdminRoleName = "admin";
        public const string DeleteRoleName = "delete";
        public const string NormalRoleName = "normal";
        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        //관계된 데이터 모두 지울지 결정
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    //builder.Entity<RawUpload>().HasOne(x => x.Config).WithOne().OnDelete(DeleteBehavior.Restrict);//참조 무결성 오류
        //    builder.Entity<RawUpload>().HasOne(x => x.Config).WithOne().OnDelete(DeleteBehavior.Cascade);//연결된 데이터 모두삭제
        //}

        //public DbSet<DutAnalysisModelBase> ChipData { get; set; }
        //public DbSet<LotModel> Lot { get; set; }
        //public DbSet<PigtailReport> PigtailReport { get; set; }
        //public DbSet<RawDataModel> RawDataModel { get; set; }
        //public DbSet<AnalysisModel> Analysis { get; set; }
        //public DbSet<PigtailReportFormat> PigtailReportFormat { get; set; }
        //public DbSet<AnalysisDataModel> AnalysisData { get; set; }
        //public DbSet<RawUpload> RawUpload { get; set; }
        //public DbSet<ConfigModel> ConfigModel { get; set; }

    }
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
}
