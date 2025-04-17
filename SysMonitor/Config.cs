using System;

namespace MonitorTray
{
    public class Config
    {
        public string Language { get; set; } = "ru";
        public string Hotkey { get; set; } = "F9";
        public string Modifier { get; set; } = "None";
        public int UpdateInterval { get; set; } = 1000; 
        public int OverlayX { get; set; } = 100;
        public int OverlayY { get; set; } = 100;
    }
}
