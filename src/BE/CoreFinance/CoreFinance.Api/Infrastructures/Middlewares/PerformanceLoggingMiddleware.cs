//using System.Diagnostics;

//namespace CoreFinance.Api.Infrastructures.Middlewares;

//public class RequestLogging
//{
//    public string? Key { get; set; }
//    public string? RequestId { get; set; }
//    public string? Schema { get; set; }
//    public string? Method { get; set; }
//    public string? Path { get; set; }
//    public string? QueryString { get; set; }
//    public string? RequestBody { get; set; }
//}

///// <summary>
/////     The Exception Handling Middleware.
///// </summary>
//public class PerformanceLoggingMiddleware
//{
//    /// <summary>
//    /// </summary>
//    private readonly ILogger _logger;

//    private readonly RequestDelegate _next;
//    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

//    /// <summary>
//    ///     Initializes a new instance of the <see cref="PerformanceLoggingMiddleware" /> class.
//    /// </summary>
//    /// <param name="logger"></param>
//    /// <param name="next">The next.</param>
//    public PerformanceLoggingMiddleware(ILogger<PerformanceLoggingMiddleware> logger,
//        RequestDelegate next)
//    {
//        _logger = logger;
//        _next = next;
//        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
//    }

//    /// <summary>
//    ///     Invokes the asynchronous.
//    /// </summary>
//    /// <param name="context">The HTTP context.</param>
//    public async Task InvokeAsync(HttpContext context)
//    {
//        context.Request.EnableBuffering();
//        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
//        await context.Request.Body.CopyToAsync(requestStream);
//        var formatRequest = new RequestLogging
//        {
//            Key = "PERF",
//            RequestId = context.TraceIdentifier,
//            Schema = context.Request.Scheme,
//            Method = context.Request.Method,
//            Path = context.Request.Path,
//            QueryString = context.Request.QueryString.ToString(),
//            RequestBody = ReadStreamInChunks(requestStream)
//        };
//        _logger.LogInformation(formatRequest.TryParseToString());

//        context.Request.Body.Position = 0;

//        _logger.LogInformation(
//            $"[PERF] - RequestId - [{context.TraceIdentifier}] - Method {context.Request.Path} started");
//        _logger.LogInformation(
//            $"[PERF] - RequestId - [{context.TraceIdentifier}] - Method {context.Request.Path}",
//            context.Request);

//        var stopwatch = Stopwatch.StartNew();

//        await _next(context);

//        stopwatch.Stop();
//        _logger.LogInformation(
//            $"[PERF] - RequestId - [{context.TraceIdentifier}] - Method {context.Request.Path} - In [{stopwatch.ElapsedMilliseconds}] milliseconds completed.");
//    }

//    private static string ReadStreamInChunks(Stream stream)
//    {
//        const int readChunkBufferLength = 4096;
//        stream.Seek(0, SeekOrigin.Begin);
//        using var textWriter = new StringWriter();
//        using var reader = new StreamReader(stream);
//        var readChunk = new char[readChunkBufferLength];
//        int readChunkLength;
//        do
//        {
//            readChunkLength = reader.ReadBlock(readChunk,
//                0,
//                readChunkBufferLength);
//            textWriter.Write(readChunk, 0, readChunkLength);
//        } while (readChunkLength > 0);

//        return textWriter.ToString();
//    }
//}