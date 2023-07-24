using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LostFound.Authorize
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _isAdmin; 
        public CustomAuthorize(params string[] IsAdmin)
        {
            _isAdmin = IsAdmin.FirstOrDefault() ?? "true";
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var user = context.HttpContext.User;
            var isAdmin = user.Claims.FirstOrDefault(t => t.Type == "IsAdmin").Value;
            if (!isAdmin.ToLower().Equals(_isAdmin)) context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
    
}
