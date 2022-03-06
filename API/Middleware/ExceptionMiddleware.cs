using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        //RequestDelegate - for next middleware pipleine
        //ILogger - to log message in console 
        //IHostEnvironment - to check envirmnet - dev or prod
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) //its in context og http
        {
            try
            {
                await _next(context);//pass to next middleware, this should be top of middlewares,
                //when any excpetion occur in middleware, then it moves up to middleware pipline,and here exception will be handled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);//log message in terminal

                //write into http context
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //we have to check enviroment , dev or prod,
                var response = _env.IsDevelopment() 
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };//json goes normal format with camelcase

                var json = JsonSerializer.Serialize(response, options);//serialise reponse 

                await context.Response.WriteAsync(json);//pass json to client
            }
        }
    }
}
