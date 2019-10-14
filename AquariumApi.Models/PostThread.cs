using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class PostThread
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public PrivacyTypes Privacy { get; set; }
        public DateTime Timestamp { get; set; }
        public int BoardId { get; set; }

        [ForeignKey("AuthorId")]
        public AquariumUser Author { get; set; }
        [ForeignKey("BoardId")]
        public PostBoard Board { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}