using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblPostComment")]
    public class PostComment
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int PostId { get; set; }

        [ForeignKey("AuthorId")]
        public AquariumUser Author { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
    }
}