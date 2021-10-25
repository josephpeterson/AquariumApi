using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class BaseException : Exception
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public ExceptionTypes Type { get; set; }
        public new Exception Source { get; set; }
        public DateTime Date { get; set; }
        public bool Resolved { get; set; } = false;
        public object Test { get; set; }
        public BaseException(string message = null): base(message)
        {
            Date = DateTime.UtcNow;
        }
    }
}