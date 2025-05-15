using System.Diagnostics;
using Castle.DynamicProxy;

namespace CoreFinance.Api.Infrastructures.Interceptors;

public class MonitoringInterceptor(
    ILogger<MonitoringInterceptor> logger,
    IHttpContextAccessor httpContextAccessor)
    : AsyncTimingInterceptor
{
    private readonly ILogger _logger = logger;

    protected override void CompletedTiming(IInvocation invocation, Stopwatch stopwatch)
    {
        var requestCtx = InitRequest();
        _logger.LogInformation(
            $"[PERF] - RequestId - [{requestCtx.Item1}] - Method {ToStringInvocation(invocation)} completed in {stopwatch.ElapsedMilliseconds}ms");
    }

    protected override void StartingTiming(IInvocation invocation)
    {
        var requestCtx = InitRequest();
        _logger.LogInformation(
            $"[PERF] - RequestId - [{requestCtx.Item1}] - Method {ToStringInvocation(invocation)} invoked!");
    }

    private Tuple<string?, DateTime> InitRequest()
    {
        var startTime = DateTime.UtcNow;
        var request = httpContextAccessor.HttpContext;
        try
        {
            if (request != null)
            {
                if (request.Items.TryGetValue("_RequestStartedAt", out var item))
                    startTime = (DateTime)item!;
                else
                    request.Items["_RequestStartedAt"] = startTime;
            }
        }
        catch
        {
            // ignored
        }

        return Tuple.Create(request?.TraceIdentifier, startTime);
    }

    private string ToStringInvocation(IInvocation invocation)
    {
        return invocation.MethodInvocationTarget != null
            ? $"{invocation.MethodInvocationTarget.ReflectedType?.FullName}.{invocation.MethodInvocationTarget.Name}"
            : string.Empty;
    }
}