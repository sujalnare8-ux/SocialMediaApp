namespace SocialMediaApp.Api.Models;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Which post this comment belongs to
    public int PostId { get; set; }
    public Post? Post { get; set; }

    // Who wrote the comment
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}