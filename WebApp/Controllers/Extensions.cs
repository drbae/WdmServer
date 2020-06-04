using DrBAE.WdmServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universe.Web.Data;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace DrBAE.WdmServer.WebApp.Controllers
{

    public static class Extensions
    {
        public static List<ConfigModel> GetConfigs(this DynamicDbContext ddc, ConfigType type)
            => ddc.Set<ConfigModel>().Where(x => x.ConfigType == type).ToList();

        //testing...
        //public static string DisplayNameFor<TModelItem, TResult>(this IHtmlHelper<IEnumerable<TModelItem>> htmlHelper,
        //    Expression<Func<TModelItem, TResult>> expression) where TModelItem: IModelBase
        //{
        //    var model = htmlHelper.ViewData.Model.FirstOrDefault();
        //    var names = model?.GetDisplayNames();
        //}

    }//class
}
