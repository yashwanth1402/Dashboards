using Microsoft.AspNetCore.Mvc.Filters;

namespace Dashboards.Server.Filters
{
    /// <summary>
    /// Logs all controller action executions for debugging/auditing.
    /// Equivalent to FFI.API/Filters/GlobalActionFilter.cs
    /// </summary>
    public class GlobalActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<GlobalActionFilter> _logger;

        public GlobalActionFilter(ILogger<GlobalActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;

            _logger.LogInformation("[REQUEST] {Controller} -> {Action}", controller, action);

            var result = await next();

            if (result.Exception != null)
            {
                _logger.LogError(result.Exception, "[ERROR] {Controller} -> {Action}", controller, action);
            }
        }
    }
}
