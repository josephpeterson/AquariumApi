using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblAccount")]
    public class AquariumUser
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime SeniorityDate { get; set; }
    }
}