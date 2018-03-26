namespace ServiceLauncher.Data
{
    using System.Collections.Generic;

    public class Settings
    {
        public string ServiceName { get; set; } = string.Empty;
        public List<string> ServicesHistory { get; } = new List<string>();
        public double Opacity { get; set; } = 1;
        public bool ShowTitleEnabled { get; set; } = true;
    }
}
