using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblBugReports")]
    public class BugReport
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public int ImpactedUserId { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string UrlLocation { get; set; }
        public string Type { get; set; }
        public string LogFile { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("ImpactedUserId")]
        public virtual AquariumUser ImpactedUser {get; set;}
    }
}