using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AquariumApi.Models
{
    public class LED
    {
            [Required]
            public int Id { get; set; }

            [MaxLength(255)]
            public int R { get; set; } = 0;
            [MaxLength(255)]
            public int G { get; set; } = 0;
            [MaxLength(255)]
            public int B { get; set; } = 0;

        public string Serialize()
        {
            //ID1R200G0B0

            string str = "ID";
            if (Id < 10)
                str += "00" + Id;
            else if (Id < 100)
                str += "0" + Id;
            else str += Id;

            str += "R";
            if (R < 10)
                str += "00" + R;
            else if (R < 100)
                str += "0" + R;
            else str += R;

            str += "G";
            if (G < 10)
                str += "00" + G;
            else if (G < 100)
                str += "0" + G;
            else str += G;

            str += "B";
            if (B < 10)
                str += "00" + B;
            else if (B < 100)
                str += "0" + B;
            else str += B;

            return str;
        }
    }
    public class LightingConfiguration
    {
        public List<LED> LedData { get; set; }
    }
}
