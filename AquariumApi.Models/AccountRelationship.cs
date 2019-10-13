using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public enum RelationshipTypes
    {
        None = 0,
        Following = 1,
        Blocked = 2,
    }
    public class AccountRelationship
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TargetId { get; set; }
        public RelationshipTypes Relationship { get; set; }
    }
}