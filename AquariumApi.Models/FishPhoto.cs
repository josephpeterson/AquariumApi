using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblPhotoFish")]
    public class FishPhoto
    {
        [Required]
        public int Id { get; set; }
        public int FishId { get; set; }
        public int PhotoId { get; set; }
        public virtual Fish Fish{ get; set; }
        public virtual PhotoContent Photo { get; set; }
    }
}
