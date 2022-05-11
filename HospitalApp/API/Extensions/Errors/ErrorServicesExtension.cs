using API.Extensions.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions
{
    public static class ErrorServicesExtension
    {
        public static IServiceCollection AddErrorServices(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(opts =>
            {
                opts.InvalidModelStateResponseFactory = actionCtx =>
                {
                    var errs = actionCtx.ModelState
                        .Where(x => x.Value!.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    var errRes = new ApiValidationErrorResponse
                    {
                        Errors = errs
                    };

                    return new BadRequestObjectResult(errRes);
                };
            });

            return services;
        }
    }
}
