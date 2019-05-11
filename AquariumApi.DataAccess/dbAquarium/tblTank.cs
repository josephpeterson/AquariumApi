using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.DataAccess
{
    [Table("tblTank")]
    public partial class tblTank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gallons { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
    }
}
