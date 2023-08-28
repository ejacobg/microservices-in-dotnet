using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;

namespace Microservice.Logging
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// UseLogging configures Serilog as the logger and adds the trace ID to each log.
        /// Use this method on the host builder in your Program.cs file.
        /// Package this library using the following command:
        ///     dotnet pack --configuration Release
        /// </summary>
        public static IHostBuilder UseLogging(this IHostBuilder builder) =>
            builder.UseSerilog((context, logger) => // Configure Serilog as the logger.
            {
                logger
                    .Enrich.FromLogContext() // Add metadata to logs.
                    .Enrich.WithSpan();
                if (context.HostingEnvironment.IsDevelopment())
                    logger.WriteTo.ColoredConsole(
                        // Add tracing information when developing.
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss} {TraceId} {Level:u3} {Message}{NewLine}{Exception}");
                else
                    logger.WriteTo.Console(new JsonFormatter()); // Use JSON logs in production.
            });
    }
}