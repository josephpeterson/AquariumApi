using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class SignupRequest
    {
        public int Id { get; set; }
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string Username { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string Password2 { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        public virtual AquariumUser Account { get; set; }
    }
}
