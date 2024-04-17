using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ETicaretAPI.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors= context.ModelState
                        .Where(x => x.Value.Errors.Any())//hiç hata var mı?
                        .ToDictionary(e => e.Key, e => e.Value.Errors.Select(e => e.ErrorMessage))//hatanın key ve mesajını alıyoruz
                        .ToArray();

                context.Result = new BadRequestObjectResult(errors);
                return;
            }

            await next();
        }
    }
}
