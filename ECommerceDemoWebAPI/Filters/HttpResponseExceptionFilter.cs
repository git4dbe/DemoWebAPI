using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace ECommerceDemoWebAPI.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                _logger.Error(context.Exception);
            }
        }
    }
}
