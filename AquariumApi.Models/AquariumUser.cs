﻿using System;
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
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } /* Todo: Remove this field from this model and move it into AquariumSignupAccount, thus no longer exposing PII */
        public DateTime SeniorityDate { get; set; }
        public string Role { get; set; }

        [ForeignKey("OwnerId")]
        public ICollection<Aquarium> Aquariums { get; set; }
    }
}