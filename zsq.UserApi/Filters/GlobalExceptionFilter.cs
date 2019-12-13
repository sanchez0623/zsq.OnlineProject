using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using zsq.UserApi.Infrastructure;

namespace zsq.UserApi.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private IHostingEnvironment _env;
        private ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(IHostingEnvironment env,ILogger<GlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var errorMessage = new JsonErrorResponse();
            if (context.Exception.GetType() == typeof(UserException))
            {
                errorMessage.Message = context.Exception.Message;
                context.Result = new BadRequestObjectResult(errorMessage);
            }
            else
            {
                errorMessage.Message = "未知异常";

                if (_env.IsDevelopment())
                {
                    errorMessage.DevelopMessage = context.Exception.StackTrace;
                }

                context.Result = new InternalServerErrorObjectResult(errorMessage);
            }

            _logger.LogError(context.Exception, context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }
}
