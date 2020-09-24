using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projectarium.WebUI.ActionResults
{
    public class ForUserFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.IsInRole("User"))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
