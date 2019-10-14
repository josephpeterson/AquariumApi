using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class PostBoard
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public PrivacyTypes Privacy { get; set; }
        public DateTime Timestamp { get; set; }

        [ForeignKey("CategoryId")]
        public PostCategory Category { get; set; }
        [ForeignKey("AuthorId")]
        public AquariumUser Author { get; set; }

        public ICollection<PostThread> Threads {get; set;}
    }
}