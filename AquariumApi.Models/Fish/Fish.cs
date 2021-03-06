﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblFish")]
    public class Fish
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AquariumId { get; set; }
        public int SpeciesId { get; set; }
        public int? BreedId { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public bool Dead { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }


        [ForeignKey("SpeciesId")]
        public Species Species { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
        public FishDeath Death { get; set; }


        public FishBreeding Breed { get; set; }
        public ICollection<Feeding> Feedings { get; set; }
        public ICollection<FishPhoto> Photos { get; set; }
        public ICollection<FishSnapshot> Snapshots { get; set; }
        public ICollection<FishDisease> Disease { get; set; }
        public int? ThumbnailPhotoId { get; set; }

        [ForeignKey("ThumbnailPhotoId")]
        public FishPhoto Thumbnail { get; set; }
    }
}