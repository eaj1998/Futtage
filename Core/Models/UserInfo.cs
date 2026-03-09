namespace Futtage.Core.Models
{
    public record UserInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
    }
}