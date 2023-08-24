using Microsoft.AspNetCore.Mvc;

namespace ServiceTests.Mocks
{
    public class NotificationsMock : ControllerBase
    {
        // Track that this controller is accessed at least once.
        public static bool ReceivedNotification = false;

        [HttpPost("/notify")]
        // Return a hardcoded response for all requests.
        public OkResult Notify()
        {
            ReceivedNotification = true;
            return Ok();
        }
    }
}