using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            // the claim thing works because of the Token !!!!!
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var id = resultContext.HttpContext.User.GetUserId();
            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user  = await unitOfWork.UserRepository.GetUserByIdAsync(id);
            user.LastActive = DateTime.UtcNow;
            await unitOfWork.Complete();
        }
    }
}
