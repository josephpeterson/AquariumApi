namespace AquariumApi.Models
{
    public class PhotoTimelapseOptions
    {
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public int Width { get; set; } = 400;
        public int Height { get; set; } = 600;
        public int Framerate { get; set; } = 12;
        public string fileType { get; set; } = "avi";
    }
}
