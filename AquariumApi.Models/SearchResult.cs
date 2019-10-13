using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class SearchResult
    {
        public int Id { get; set; }
        public int Relevency { get; set; }
        public object Data { get; set; }
        public string Type { get; set; }
    }
}