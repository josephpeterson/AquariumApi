using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class BaseException
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public ExceptionTypes Type { get; set; }
        public Exception Source { get; set; }
        public DateTime Date { get; set; }
        public bool Resolved { get; set; }
        public object Test { get; set; }
    }
}