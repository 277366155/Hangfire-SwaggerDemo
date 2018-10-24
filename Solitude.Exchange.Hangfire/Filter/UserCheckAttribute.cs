using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Solitude.Exchange.Hangfire
{
    public class UserCheckAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var auth = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(auth))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult("请先登录");
            }
            if (auth!= "Bearer boo")
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                context.Result = new JsonResult("身份错误");                
            }
        }
    }
}
