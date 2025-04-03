using Microsoft.AspNetCore.Diagnostics;
using SothbeysKillerApi.Exceptions;

namespace SothbeysKillerApi.ExceptionHandlers;

public class AuctionValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is AuctionValidationException ex)
        {
            httpContext.Response.StatusCode = 400;

            await httpContext.Response
                .WriteAsJsonAsync(new
                    {
                        target = ex.Field, 
                        description = ex.Description
                    },
                cancellationToken);

            return true;
        }

        return false;
    }
}