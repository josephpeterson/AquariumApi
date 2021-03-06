﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblPhotoAquarium")]
    public class AquariumPhoto
    {
        [Required]
        public int Id { get; set; }
        public int AquariumId { get; set; }
        public int PhotoId { get; set; }
        public virtual Aquarium Aquarium { get; set; }
        public virtual PhotoContent Photo { get; set; }
    }
}
