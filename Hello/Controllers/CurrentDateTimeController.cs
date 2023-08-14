using System;
using Microsoft.AspNetCore.Mvc;

namespace Hello.Controllers
{
    public class
        CurrentDateTimeController : ControllerBase // Inheriting from ControllerBase declares this class as an MVC controller.
    {
        [HttpGet("/")] // Attach this handler to the "/" path.
        public object Get() => DateTime.UtcNow;
    }
}