using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class PostCategory
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public PrivacyTypes Privacy { get; set; }

        public ICollection<PostBoard> Boards { get; set; }
    }
}