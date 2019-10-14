using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblPost")]
    public class Post
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int Privacy { get; set; }
        public DateTime Timestamp { get; set; }
        public int ThreadId { get; set; }

        [ForeignKey("AuthorId")]
        public AquariumUser Author { get; set; }
        [ForeignKey("ThreadId")]
        public PostThread Thread { get; set; }
    }
}