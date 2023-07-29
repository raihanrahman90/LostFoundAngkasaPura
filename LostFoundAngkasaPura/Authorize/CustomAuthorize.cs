using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DTO.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Extensions;

namespace LostFound.Authorize
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly bool _isAdmin;
        private readonly bool _superAdminOnly;
        public CustomAuthorize(bool IsAdmin = false, bool superAdminOnly = false)
        {
            _isAdmin = IsAdmin;
            _superAdminOnly = superAdminOnly;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var user = context.HttpContext.User;

            var tokenIsAdmin = user.Claims.FirstOrDefault(t => t.Type == "IsAdmin").Value;
            if (_isAdmin && !tokenIsAdmin.ToLower().Equals("true"))
                throw new NotAuthorizeError();

            var access = user.Claims.FirstOrDefault(t => t.Type == "Access").Value;
            if (_superAdminOnly && !access.ToLower().Equals("superadmin"))
                throw new NotAuthorizeError();
        }
    }
    
}
