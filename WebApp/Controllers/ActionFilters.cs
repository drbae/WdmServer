using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    public class OptionalParametersAttribute : ActionFilterAttribute
    {
        private string[] _params;

        public OptionalParametersAttribute(params string[] props)
        {
            _params = props;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var property in _params)
            {
                context.ModelState.Remove(property);

                //var arg = context.ActionArguments[property];
                //var p = arg.GetType().GetProperty(property) ?? throw new Exception($"property '{property}' not found");
                //var dv = _default(p.PropertyType);
                //var v = p.GetValue(arg);
                //if (v == null || v.Equals(dv)) context.ModelState.TryAddModelError(property, property + " is required.");
            }

            //object? _default(Type type) => type.IsPrimitive ? Activator.CreateInstance(type) : null;
        }

    }//class
}
