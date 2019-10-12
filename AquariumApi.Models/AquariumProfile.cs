using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblAquariumProfile")]
    public class AquariumProfile
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public int stars { get; set; }

        [ForeignKey("Id")]
        public AquariumUser Account { get; set; }

        [ForeignKey("OwnerId")]
        public virtual ICollection<Aquarium> Aquariums { get; set; }

        [NotMapped]
        public virtual ICollection<Fish> Fish { get; set; }
        [NotMapped]
        public virtual ICollection<Activity> Activity { get; set; }
        //[NotMapped]
        public string Thumbnail { get; set; }
    }
}