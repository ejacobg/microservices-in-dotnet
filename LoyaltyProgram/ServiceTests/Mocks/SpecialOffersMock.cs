using Microsoft.AspNetCore.Mvc;

namespace ServiceTests.Mocks
{
    public class SpecialOffersMock : ControllerBase
    {
        [HttpGet("/specialoffers/events")]
        // Return a hardcoded response for all requests.
        public ActionResult<object[]> GetEvents([FromQuery] int start, [FromQuery] int end) =>
            new[]
            {
                new
                {
                    SequenceNumber = 1,
                    Name = "baz",
                    Content = new
                    {
                        OfferName = "foo",
                        Description = "bar",
                        Item = new { ProductName = "name" }
                    }
                }
            };
    }
}