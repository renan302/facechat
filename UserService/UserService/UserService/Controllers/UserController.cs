using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Context;
using UserService.Models;
using UserService.Repositories;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repository;
        private Int64 myId;

        public UserController(UserContext context)
        {
            _repository = new UserRepository(context);

            myId = 0;
        }

        public void PopulateVariables()
        {
            if (User != null)
            {
                myId = long.Parse(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                       .Select(c => c.Value).SingleOrDefault());
            }
        }
        
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserClass()
        {
            PopulateVariables();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            var userClass = await _repository.GetUserById(myId);

            if (userClass == null)
            {
                return NotFound();
            }

            return Ok(userClass);
        }
        
        
        [HttpPut]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PutUserClass([FromBody] UserClass userClass)
        {
            PopulateVariables();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userClass.UserId = myId;

            try
            {
                await _repository.UpdateUser(userClass);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound();
            }            

            return Ok();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUserClass([FromBody] UserClass userClass)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                userClass = await _repository.InsertUser(userClass);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }            

            return Ok(new { Secret = userClass.Secret });
        }
        
        [HttpDelete]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteUserClass()
        {
            PopulateVariables();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userClass = await _repository.GetUserById(myId);

            if (userClass == null)
            {
                return NotFound();
            }

            userClass.Excluded = 1;
            
            await _repository.UpdateUser(userClass);

            return Ok();
        }

        [HttpGet]
        [Route("Authenticate/{username}/{password}")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromRoute] string username, [FromRoute] string password)
        {

            var user = await _repository.GetUserByUserPass(username, password);

            if (user == null)
            {
                return NotFound(new { message = "User or Password Invalid!" });
            }

            var token = TokenService.GenerateToken(user);

            return Ok(new
            {
                Token = token
            });
        }

        [HttpGet]
        [Route("RefreshToken")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<dynamic>> RefreshToken()
        {
            PopulateVariables();

            var user = await _repository.GetUserById(myId);

            if (user == null)
            {
                return NotFound();
            }

            var token = TokenService.GenerateToken(user);

            return Ok(new
            {
                Token = token
            });
        }

    }
}