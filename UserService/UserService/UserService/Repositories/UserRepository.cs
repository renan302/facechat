using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Context;
using UserService.Models;
using UserService.Services;

namespace UserService.Repositories
{
    public class UserRepository
    {
        private UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<UserClass> GetUserByUserPass(string username, string pass)
        {
            return await _context.User.Where(
                user => user.User == username 
            && user.Password == MD5Service.MD5Hash(pass) 
            && user.Excluded == 0).FirstOrDefaultAsync();
        }

        public async Task<UserClass> GetUserBySecret(string secret)
        {
            return await _context.User.Where(user => user.Secret == secret && user.Excluded == 0).FirstOrDefaultAsync();
        }

        public async Task<UserClass> GetUserById(Int64 id)
        {
            return await _context.User.Where(user => user.UserId == id && user.Excluded == 0).FirstOrDefaultAsync();
        }

        public async Task<UserClass> GetUserByUser(string username)
        {
            return await _context.User.Where(user => user.User == username && user.Excluded == 0).FirstOrDefaultAsync();
        }

        public bool UserClassExists( string secret)
        {
            return _context.User.Any(e => e.Secret == secret);
        }

        public async Task UpdateUser(UserClass userClass)
        {

            UserClass userClassValidate = await GetUserById(userClass.UserId);

            if (userClassValidate == null)
            {
                throw new KeyNotFoundException();
            }

            userClass.ModifiedAt = DateTime.Now;
            _context.Entry(userClass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserClassExists(userClass.Secret))
                {
                    throw new KeyNotFoundException();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<UserClass> InsertUser(UserClass userClass)
        {
            userClass.CreateAt = DateTime.Now;
            userClass.ModifiedAt = DateTime.Now;
            userClass.Password = MD5Service.MD5Hash(userClass.Password);

            UserClass userClassValidate = await GetUserByUser(userClass.User);

            if (userClassValidate != null)
            {
                throw new Exception("User already exists!");
            }

            _context.User.Add(userClass);
            await _context.SaveChangesAsync();
            userClass.Secret = AppService.SecretGenerate(userClass.UserId).ToUpper();
            _context.Update(userClass);
            await _context.SaveChangesAsync();

            return userClass;
        }
    }
}
