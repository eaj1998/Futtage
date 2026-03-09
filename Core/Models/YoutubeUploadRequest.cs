namespace Futtage.Core.Models
{
    public record YouTubeUploadRequest
    {
        public string VideoFilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;
        public bool IsForKids { get; set; }
        public string Privacy { get; set; } = "private"; // private, public, unlisted
        public string[] Tags { get; set; } = Array.Empty<string>();
        public string Category { get; set; } = "22";
    }
}