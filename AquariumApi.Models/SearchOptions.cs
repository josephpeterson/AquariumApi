using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class SearchOptions
    {
        public int AccountId { get; set; }
        public bool Aquariums { get; set; }
        public bool Accounts { get; set; }
        public bool Photos { get; set; }
        public bool Fish { get; set; }
        public string Query { get; set; }
        public bool Species { get; set; }
        public bool Threads { get; set; }
        public bool Posts { get; set; }
    }
}