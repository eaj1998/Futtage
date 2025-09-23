using System;
using System.IO;

namespace Futtage.Infrastructure.Configuration
{
    public class AppSettings
    {
        public string FFmpegPath { get; set; } = "ffmpeg.exe";
        public string LoggingPath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Futtage",
            "Logs");

        public YouTubeSettings YouTube { get; set; } = new YouTubeSettings();
        public VideoSettings Video { get; set; } = new VideoSettings();
        public UISettings UI { get; set; } = new UISettings();
    }

    public class YouTubeSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = "Futtage";
        public string[] Scopes { get; set; } = { "https://www.googleapis.com/auth/youtube.upload" };
    }

    public class VideoSettings
    {
        public string DefaultOutputPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        public string TempPath { get; set; } = Path.GetTempPath();
        public string DefaultQuality { get; set; } = "copy";
        public bool DeleteTempFiles { get; set; } = true;
    }

    public class UISettings
    {
        public string Theme { get; set; } = "Light";
        public bool AutoAdvanceSteps { get; set; } = false;
        public bool ShowTooltips { get; set; } = true;
        public bool EnableAnimations { get; set; } = true;
    }
}