using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace UnitTests
{
    public class ExampleTests : IDisposable // We need to extend IDisposable since our host needs to be disposed of.
    {
        private readonly IHost _host;
        private readonly HttpClient _sut; // "System Under Test"

        public class TestController : ControllerBase
        {
            [HttpGet("/")]
            public OkResult Get() => Ok();

        }

        public ExampleTests()
        {
            _host = new HostBuilder() // Build and run a server to be tested.
                .ConfigureWebHost(host =>
                    host
                        .ConfigureServices(x => x.AddControllersByType(typeof(TestController))) // Use our custom extension method to only register the TestController on the server.
                        .Configure(x => x.UseRouting().UseEndpoints(opt => opt.MapControllers())) // Map all endpoints on the TestController.
                        .UseTestServer())
                .Start();
            _sut = _host.GetTestClient();
        }

        [Fact]
        public async Task TestRootResponse()
        {
            var actual = await _sut.GetAsync("/");
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        }

        public void Dispose()
        {
            _host?.Dispose();
            _sut?.Dispose();
        }
    }
}