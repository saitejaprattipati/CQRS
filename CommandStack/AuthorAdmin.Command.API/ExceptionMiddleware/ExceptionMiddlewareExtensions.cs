using Author.Core.Framework;
using Author.Core.Framework.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Diagnostics;
using System.Reflection;

namespace AuthorAdmin.Command.API.ExceptionMiddleware
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IUtilityService utilityService)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    // the IsTrusted() extension method doesn't exist and
                    // you should implement your own as you may want to interpret it differently
                    // i.e. based on the current principal

                    var errorDetail = utilityService.IsTrusted() ? exception.Demystify().ToString() : "The instance value should be used to identify the problem when calling customer support";

                    var problemDetails = new ProblemDetails
                    {
                        Instance = $"urn:tax@handAPI:error:{Guid.NewGuid()}"
                    };

                    if (exception is BadHttpRequestException badHttpRequestException)
                    {
                        problemDetails.Title = "Invalid request";
                        problemDetails.Status = (int)typeof(BadHttpRequestException).GetProperty("StatusCode",
                            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(badHttpRequestException);
                        problemDetails.Detail = badHttpRequestException.Message;
                    }
                    else
                    {
                        problemDetails.Title = "An unexpected error occurred!";
                        problemDetails.Status = 500;
                        problemDetails.Detail = errorDetail;//exception.Demystify().ToString();
                    }

                    // log the exception etc..

                    context.Response.StatusCode = problemDetails.Status.Value;
                    context.Response.WriteJson(problemDetails, "application/problem+json");
                });
            });
        }
    }

}
