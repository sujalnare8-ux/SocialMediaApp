namespace SocialMediaApp.Api.Models;

public class Post
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Who wrote the post
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    // Related data
    public List<Comment> Comments { get; set; } = new();
    public List<Like> Likes { get; set; } = new();
}
