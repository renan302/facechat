using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Context;
using UserService.Exceptions;
using UserService.Models;

namespace UserService.Repositories
{
    public class UserFriendRepository
    {
        private UserContext _context;
        private UserRepository _userRepository;

        public UserFriendRepository(UserContext context)
        {
            _context = context;
            _userRepository = new UserRepository(context);
        }
        public async Task<DateTime> GetLastModifiedAt(Int64 myid)
        {
            return await _context.UserFriendClass
                .Where(user => user.UserId == myid)    
                .MaxAsync(x => x.ModifiedAt);
        }

        public async Task AddFriend(Int64 myid, string username)
        {
            
            var friendUserClass = await _userRepository.GetUserByUser(username);

            if (friendUserClass.UserId == myid)
            {
                throw new InvalidUserException();
            }
         
            if (friendUserClass == null)
            {
                throw new Exception();
            }

            UserFriendClass userFriendValidate = await _context.UserFriendClass.
                    Where(x => x.UserId == myid && x.UserIdFriend == friendUserClass.UserId && x.Excluded == 0).
                    FirstOrDefaultAsync();

            if (userFriendValidate != null)
            {
                throw new UserFriendExistException();
            }

            var userFriendClass = new UserFriendClass()
            { Blocked = false, UserIdFriend = friendUserClass.UserId, UserId = myid, 
                CreateAt = DateTime.Now, ModifiedAt = DateTime.Now, Excluded = 0 };                 

            _context.UserFriendClass.Add(userFriendClass);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteFriend(Int64 myid, string username)
        {
            
            var friendUserClass = await _userRepository.GetUserByUser(username);

            if (friendUserClass == null)
            {
                throw new Exception();
            }

            var userFriendClass = await _context.UserFriendClass.
                             Where(userfriend => userfriend.UserIdFriend == friendUserClass.UserId
                             && userfriend.UserId == myid && userfriend.Excluded == 0).FirstOrDefaultAsync();

            if (userFriendClass == null)
            {
                throw new Exception();
            }
            userFriendClass.ModifiedAt = DateTime.Now;
            userFriendClass.Excluded = 1;
            _context.Entry(userFriendClass).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
        public async Task BlockFriend(Int64 myid, string username)
        {
           
            var friendUserClass = await _userRepository.GetUserByUser(username);

            if (friendUserClass == null)
            {
                throw new Exception();
            }

            var userFriendClass = await _context.UserFriendClass.
                             Where(userfriend => userfriend.UserIdFriend == friendUserClass.UserId
                             && userfriend.UserId == myid && userfriend.Excluded == 0).FirstOrDefaultAsync();

            if (userFriendClass == null)
            {
                throw new Exception();
            }

            userFriendClass.ModifiedAt = DateTime.Now;
            userFriendClass.Blocked = true;

            _context.UserFriendClass.Update(userFriendClass);
            await _context.SaveChangesAsync();
        }
        public async Task UnblockFriend(Int64 myid, string username)
        {
            
            var friendUserClass = await _userRepository.GetUserByUser(username);

            if (friendUserClass == null)
            {
                throw new Exception();
            }

            var userFriendClass = await _context.UserFriendClass.
                             Where(userfriend => userfriend.UserIdFriend == friendUserClass.UserId
                             && userfriend.UserId == myid && userfriend.Excluded == 0).FirstOrDefaultAsync();

            if (userFriendClass == null)
            {
                throw new Exception();
            }

            userFriendClass.ModifiedAt = DateTime.Now;
            userFriendClass.Blocked = false;

            _context.UserFriendClass.Update(userFriendClass);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Object>> GetFriends(Int64 myid)
        {            
            return await _context.UserFriendClass
                .Where(user => user.UserId == myid && user.Excluded == 0)
                .Include(x => x.FriendUser)
                .Select(x => new { User = x.FriendUser.User, Name = x.FriendUser.Name, Blocked = x.Blocked})
                .ToListAsync();
        }
    }
}
