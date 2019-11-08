using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblPhotoContent")]
    public class PhotoContent
    {
        [Required]
        public int Id { get; set; }
        public string Filepath { get; set; }
        public bool Exists { get; set; } = false;
        public DateTime Date { get; set; }

    }
}
