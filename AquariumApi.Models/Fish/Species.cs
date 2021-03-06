﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblSpecies")]
    public class Species
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public int? TemperatureMin { get; set; }
        public int? TemperatureMax { get; set; }
        public int? Lifespan { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string CareLevel { get; set; }
        public string Thumbnail { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumGallons { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PhMin { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PhMax { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxSize { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        //Aggregate fields
        [NotMapped]
        public int AquariumCount { get; set; }
        [NotMapped]
        public int FishCount { get; set; }
    }
}