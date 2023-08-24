using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiceTests.Mocks
{
    /// <summary>
    /// MocksHost is used to serve mocked controllers on a separate thread.
    /// </summary>
    public class MocksHost : IDisposable
    {
        private readonly IHost _hostForMocks;

        public MocksHost(int port)
        {
            _hostForMocks =
                Host.CreateDefaultBuilder() // Create a new host to serve our controllers.
                    .ConfigureWebHostDefaults(x => x
                        .ConfigureServices(services => services.AddControllers()) // Add our mock controllers.
                        .Configure(app => app.UseRouting().UseEndpoints(opt => opt.MapControllers()))
                        .UseUrls($"http://localhost:{port}")) // Serve on the given port.
                    .Build();

            new Thread(() => _hostForMocks.Run()).Start(); // Run the host on a separate thread.
        }

        public void Dispose() => _hostForMocks.Dispose();
    }
}