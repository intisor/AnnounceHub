using Microsoft.AspNetCore.Authorization;

namespace AnnounceHub
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,Inherited = false)]
    public class Auth : AuthorizeAttribute
    {
        public string? Username { get; set; }
        protected bool AuthorizeCore(HttpContextAccessor httpContextAccessor) 
        {
            var username = httpContextAccessor.HttpContext!.User.Identity!.Name;
            return username == Username;
        }
    }
}
