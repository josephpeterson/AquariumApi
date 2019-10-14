using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class PostReaction
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostId { get; set; }
        public int Reaction { get; set; }

        [ForeignKey("AuthorId")]
        public AquariumUser Author { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
    }
}