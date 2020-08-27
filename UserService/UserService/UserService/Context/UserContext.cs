using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Context
{
    public class UserContext : DbContext
    {
        public DbSet<UserClass> User { get; set; }

        public UserContext(DbContextOptions<UserContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<UserService.Models.UserFriendClass> UserFriendClass { get; set; }
    }
}