using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text;

namespace AquariumApi.Models
{
    public class CameraConfiguration
    {
        public int Id { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
        int Height { get; set; }
        int Width { get; set; }
        int Sharpness { get; set; }
        int Contrast { get; set; }
        int Saturation { get; set; }
        int Brightness { get; set; }
        int Iso { get; set; }
        bool HFlip { get; set; }
        bool VFlip { get; set; }
        Vector3 Roi { get; set; }
        int Rotation { get; set; }
    }
}
