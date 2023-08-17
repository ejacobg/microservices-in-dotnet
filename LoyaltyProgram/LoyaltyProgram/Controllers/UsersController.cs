using System;
using System.Collections.Generic;
using LoyaltyProgram.Models;
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
        public User UpdateUser(int userId, [FromBody] User user)
            => RegisteredUsers[userId] = user; // The user IDs can technically be edited, but we will be ignoring that for now.

        private User RegisterUser(User user)
        {
            var userId = RegisteredUsers.Count;
            return RegisteredUsers[userId] = user with { Id = userId };
        }
    }
}