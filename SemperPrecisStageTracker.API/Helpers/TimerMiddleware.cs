using System.Diagnostics;

namespace SemperPrecisStageTracker.API.Helpers
{
    public class TimerMiddleware
    {
        private readonly RequestDelegate _next;
        public TimerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            Stopwatch sw = Stopwatch.StartNew();
            httpContext.Response.OnStarting(() =>
            {
                sw.Stop();
                httpContext.Response.Headers.Add("x-execution-time", sw.ElapsedMilliseconds.ToString("g"));
                return Task.CompletedTask;
            });
            try
            {
                await _next(httpContext);
            }
            finally
            {
                sw.Stop();
            }
        }
    }
}
