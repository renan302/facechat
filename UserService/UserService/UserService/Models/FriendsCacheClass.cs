using System;
using System.Collections.Generic;

namespace UserService.Models
{
    internal class FriendsCacheClass
    {
        public DateTime CreateAt { get; set; }
        public IEnumerable<Object> Friends { get; set; }
    }
}
