namespace Futtage.Core.Models
{
    public class ProcessingProgress
    {
        public int Percentage { get; }
        public string Message { get; }
        public string CurrentFile { get; set; } = string.Empty;

        public ProcessingProgress(int percentage, string message)
        {
            Percentage = Math.Max(0, Math.Min(100, percentage));
            Message = message ?? string.Empty;
        }
    }
}