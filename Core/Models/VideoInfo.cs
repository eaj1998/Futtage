using System;

namespace Futtage.Core.Models
{
    public class VideoInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double FrameRate { get; set; }
        public long FileSize { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;

        public string FormattedDuration => Duration.ToString(@"hh\:mm\:ss");
        public string FormattedFileSize => FormatFileSize(FileSize);
        public string Resolution => $"{Width}x{Height}";
        public string FileName => System.IO.Path.GetFileName(FilePath);

        private static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }
    }
}