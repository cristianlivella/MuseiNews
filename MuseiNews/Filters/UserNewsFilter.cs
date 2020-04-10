using Microsoft.AspNetCore.Mvc.Filters;
using MuseiNews.DAL;
using MuseiNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MuseiNews.Filters
{
    public class UserNewsFilter : ActionFilterAttribute
    {
        private readonly NewsApiContext _context;
        public UserNewsFilter(NewsApiContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            actionContext.HttpContext.Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues clientToken);
            int clientId = 0;
            int internalUserId = 0;
            try
            {
                clientId = int.Parse(actionContext.ActionArguments["clientId"].ToString());
                internalUserId = int.Parse(actionContext.ActionArguments["internalUserId"].ToString());
            }
            catch
            {
                actionContext.Result = new Microsoft.AspNetCore.Mvc.BadRequestResult();
                return;
            }
            if (!clientToken.ToString().ToLower().Contains("bearer"))
            {
                actionContext.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
            var token = clientToken.ToString().ToLower().Replace("bearer", "").Trim();
            if (token.Length != 64)
            {
                actionContext.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
            System.Diagnostics.Debug.WriteLine(token);
            if (!_context.Clients.Any(c => c.Id == clientId && c.SecretToken == token))
            {
                actionContext.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
            var user = _context.Users.Where(u => u.ClientId == clientId && u.InternalId == internalUserId).FirstOrDefault();
            if (user == null)
            {
                user = new User();
                user.InternalId = internalUserId;
                user.ClientId = clientId;
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            actionContext.ActionArguments.Add("userId", user.Id);
        }
    }
}
