using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DrBAE.WdmServer.WebApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using DrBAE.WdmServer.Logging;
using DrBAE.WdmServer.ExceptionProcessing;
using System.Text;
using Universe.Web.Data;
using DrBAE.WdmServer.Models;

#pragma warning disable CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
namespace DrBAE.WdmServer.WebApp.Controllers
{
    public class AppControllerBase<C, M> : AppControllerBase<C> where C: AppControllerBase<C> where M: class, IModelBase
    {
        protected readonly DbSet<M> _ds;
        protected readonly List<ConfigModel> _configs;
        public AppControllerBase(IServiceProvider sp) : base(sp, typeof(M)) => _ds = _dyContext.Set<M>();
        public AppControllerBase(IServiceProvider sp, ConfigType configType) : this(sp) => _configs = _dyContext.GetConfigs(configType);
    }

    public class AppControllerBase<C> : Controller, ICheckParam<C> where C : AppControllerBase<C>
    {
        public const string ContentHeaderName = "_ContentHeader";
        public const string ContentFooterName = "_ContentFooter";

        protected readonly IServiceProvider _sp;
        //protected readonly AppDbContext _context;
        protected readonly IConfiguration _config;
        protected readonly IWebHostEnvironment _env;
        protected readonly ILogger<C> _logger;

        protected readonly DynamicDbContext _dyContext;
        protected readonly Type _type;

        public AppControllerBase(IServiceProvider sp, Type modelType) : this(sp) => _type = modelType;
        public AppControllerBase(IServiceProvider sp)
        {
            _sp = sp;
            //_context = sp.GetService<AppDbContext>();
            _config = sp.GetService<IConfiguration>();
            _env = sp.GetService<IWebHostEnvironment>();
            _logger = sp.GetService<ILogger<C>>();
            _dyContext = sp.GetService<DynamicDbContext>();
        }

        /// <summary>
        /// return string : url에 사용할 컨트롤러 이름
        /// </summary>
        public static string Route => typeof(C).Name.Replace("Controller", "");

        /// <summary>
        /// 주어진 모델의 주어진 속성만을 업데이트에 반영하여 DbContext를 저장한다.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        protected async Task UpdateDbContextAsync(object model, params string[] names)
        {
            var entry = _dyContext.Entry(model);
            entry.State = EntityState.Unchanged;
            foreach (var name in names) entry.Property(name).IsModified = true;
            await _dyContext.SaveChangesAsync();
        }

        //protected void logParams(MethodBase mi, params object[] paramValues) => _logParams(mi, paramValues);

        /// <summary>
        /// TODO: 호출 파라미터가 있을 경우 memberName 이 잘 들어오는지 확인
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="paramValues"></param>
        protected void logParams(string caller, params object?[] paramValues)
        {
            var values = paramValues.Select(x => x ?? new object()).ToArray();
            _logger.LogInformation(ICheckParam<C>.ParamString(caller, values));
        }
    }//class
}
#pragma warning restore CS8618 // null을 허용하지 않는 필드가 초기화되지 않았습니다. nullable로 선언하는 것이 좋습니다.
