using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Repositories;
using UserService.Services;
using UserService.Models;
using UserService.Context;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Exceptions;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFriendController : ControllerBase
    {
        private readonly UserFriendRepository _repository;
        private readonly CacheService _cache;
        private Int64 myId;       

        public UserFriendController(UserContext context, IDistributedCache cache)
        {
            _repository = new UserFriendRepository(context);

            _cache = new CacheService(cache);
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
        public async Task<IEnumerable<Object>> GetUserFriendClass()
        {
            PopulateVariables();

            try
            {
                IEnumerable<Object> friends;

                var keyCache = $"user.{myId.ToString()}.friends";

                FriendsCacheClass obj = _cache.GetValue<FriendsCacheClass>(keyCache);

                if (obj == null)
                {
                    
                    friends = await _repository.GetFriends(myId);
                    _cache.SetValue(keyCache, new FriendsCacheClass() { CreateAt = DateTime.Now, Friends = friends });

                    return friends;
                }
                else
                {
                    if (obj.CreateAt < await _repository.GetLastModifiedAt(myId))
                    {
                        friends = await _repository.GetFriends(myId);
                        _cache.SetValue(keyCache, new FriendsCacheClass() { CreateAt = DateTime.Now, Friends = friends });
                    }
                    else
                    {
                        friends = obj.Friends;
                    }

                    return friends;
                }

                
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        [HttpPost]
        [Route("AddFriend/{username}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PostUserFriendClass([FromRoute] string username)
        {
            PopulateVariables();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.AddFriend(myId, username);
            }
            catch (InvalidUserException e)
            {
                return BadRequest("Invalid User!");
            }
            catch (UserFriendExistException e)
            {
                return BadRequest("Friend already exists!");
            }
            catch (Exception e)
            {
                return NotFound();
            }            

            return Ok();
        }
        
        [HttpPost]
        [Route("BlockFriend/{username}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PostBlockUserFriendClass([FromRoute] string username)
        {
            PopulateVariables();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.BlockFriend(myId, username);
            }
            catch (Exception e)
            {
                return NotFound();
            }

            return Ok();
        }
        
        [HttpPost]
        [Route("UnblockFriend/{username}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PostUnblockUserFriendClass([FromRoute] string username)
        {
            PopulateVariables();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.UnblockFriend(myId, username);
            }
            catch (Exception e)
            {
                return NotFound();
            }

            return Ok();
        }
        
        [HttpDelete]
        [Route("{username}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteUserFriendClass([FromRoute] string username)
        {
            PopulateVariables();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.DeleteFriend(myId, username);
            }
            catch (Exception e)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}