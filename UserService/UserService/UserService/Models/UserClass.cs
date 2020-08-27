using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace UserService.Models
{
    [Table("user")]
    public class UserClass
    {
        [Key]
        public Int64 UserId { get; set; }
        [Required, MaxLength(20), MinLength(5)]
        public string User { get; set; }
        [Required, MaxLength(30), MinLength(10)]
        public string Name { get; set; }
        [Required, MaxLength(33), MinLength(8)]        
        public string Password { get; set; }
        [MaxLength(33)]
        public string Secret { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        [DefaultValue("0")]
        public int Excluded { get; set; }
    }
}
