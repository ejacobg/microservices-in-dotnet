using System;
using System.Collections.Generic;
using System.Linq;
using LoyaltyProgram.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyProgram.Controllers
{
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private static readonly Dictionary<int, User> RegisteredUsers = new();

        [HttpGet("{userId:int}")]
        public ActionResult<User> GetUser(int userId) =>
            RegisteredUsers.ContainsKey(userId)
                ? Ok(RegisteredUsers[userId])
                : NotFound();

        [HttpPost("")]
        public ActionResult<User> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest();
            var newUser = RegisterUser(user);
            
            // Respond with the Location header set, as well as the data for the new user.
            return Created(new Uri($"/users/{newUser.Id}", UriKind.Relative), newUser); 
        }

        [HttpPut("{userId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Only accept requests with a valid JWT.
        public ActionResult<User> UpdateUser(int userId, [FromBody] User user)
        {
            var hasUserId = int.TryParse(
                User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value, // Grab the user ID from the set of claims.
                out var userIdFromToken);

            // Users are only able to edit their own settings, and no one else's.
            if (!hasUserId || userId != userIdFromToken)
                return Unauthorized();

            return RegisteredUsers[userId] = user; // The user IDs can technically be edited, but we will be ignoring that for now.
        }

        private User RegisterUser(User user)
        {
            var userId = RegisteredUsers.Count;
            return RegisteredUsers[userId] = user with { Id = userId };
        }
    }
}