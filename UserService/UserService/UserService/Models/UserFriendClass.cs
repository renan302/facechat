using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models
{
    [Table("userfriend")]
    public class UserFriendClass
    {
        [Key]
        public Int64 UserFriendId { get; set; }
        [Required]
        [ForeignKey("MyUser")]
        public Int64 UserId { get; set; }
        [Required]
        [ForeignKey("FriendUser")]
        public Int64 UserIdFriend { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        [Required]
        public bool Blocked { get; set; }
        [DefaultValue("0")]
        public int Excluded { get; set; }
        public UserClass MyUser { get; set; }
        public UserClass FriendUser { get; set; }
    }
}
