using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.IO;

namespace PMAuth.Middleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<LogMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await LogRequestBody(context.Request);

            var originalBodyStream = context.Response.Body;
            var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await _next.Invoke(context);

            await LogResponseBody(context);

            await responseBody.CopyToAsync(originalBodyStream);


        }
        private async Task LogRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var encoding = GetEncodingFromContentType(request.ContentType);
            _logger.LogInformation($"Http Request Information:{Environment.NewLine}" +
                                   $"ContentType: {request.ContentType} " +
                                   $"Schema:{request.Scheme} " +
                                   $"Host: {request.Host} " +
                                   $"Path: {request.Path} " +
                                   $"QueryString: {request.QueryString} " +
                                   $"Request Body: {await ReadStreamInChunks(request.Body, encoding)}");
            request.Body.Seek(0, SeekOrigin.Begin);
        }

        private async Task LogResponseBody(HttpContext context)
        {
            var response = context.Response;
            response.Body.Seek(0, SeekOrigin.Begin);
            var encoding = GetEncodingFromContentType(response.ContentType);
            _logger.LogInformation($"Http Response Information:{Environment.NewLine}" +
                                   $"Schema:{context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"ContentType: {response.ContentType} " +
                                   $"StatusCode: {response.StatusCode} " +
                                   $"Response Body: {await ReadStreamInChunks(response.Body, encoding)}");
            response.Body.Seek(0, SeekOrigin.Begin);
        }

        private static Encoding GetEncodingFromContentType(string contentTypeStr)
        {
            if (string.IsNullOrEmpty(contentTypeStr))
            {
                return Encoding.UTF8;
            }

            ContentType contentType;
            try
            {
                contentType = new ContentType(contentTypeStr);
            }
            catch (FormatException)
            {
                return Encoding.UTF8;
            }

            if (string.IsNullOrEmpty(contentType.CharSet))
            {
                return Encoding.UTF8;
            }

            return Encoding.GetEncoding(contentType.CharSet, EncoderFallback.ExceptionFallback,
                DecoderFallback.ExceptionFallback);
        }

        private static async Task<string> ReadStreamInChunks(Stream stream, Encoding encoding)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096, leaveOpen: true);
            var text = await reader.ReadToEndAsync().ConfigureAwait(false);
            stream.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }
}
