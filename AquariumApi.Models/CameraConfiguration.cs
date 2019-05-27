using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text;

namespace AquariumApi.Models
{
    public class CameraConfiguration
    {
        public int? Id { get; set; }
        public int Height { get; set; } = 1080;
        public int Width { get; set; } = 1920;
        public int Sharpness { get; set; } = 0;
        public int Contrast { get; set; } = 0;
        public int Saturation { get; set; } = 0;
        public string ExposureMode { get; set; } = "auto";
        public int Brightness { get; set; } = 0;
        public int Iso { get; set; } = 100;
        public bool HFlip { get; set; } = false;
        public bool VFlip { get; set; } = false;
        public decimal RoiX { get; set; } = 0;
        public decimal RoiY { get; set; } = 0;
        public decimal RoiW { get; set; } = 0;
        public decimal RoiH { get; set; } = 0;
        public int Rotation { get; set; } = 180;

        public string Output;

        public override string ToString()
        {
            return $"-o '{Output}' " +
                $"-h {Height} " +
                $"-w {Width} " +
                $"-sh '{Sharpness}' " +
                $"-co '{Contrast}' " +
                $"-sa '{Saturation}' " +
                $"-br '{Brightness}' " +
                $"-ISO {Iso} " +
                $"-ev 0 " +
                $"-ex '{ExposureMode}' " +
                $"{(HFlip ? "-hf": "")} " +
                $"{(VFlip ? "-vf":"")} " +
                $"-rot {Rotation} " +
                $"-roi {RoiX},{RoiY},{RoiW},{RoiH} ";
        }
    }
}
