using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class AccountRelationship
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TargetId { get; set; }
        public int Relationship { get; set; }
    }
}