namespace SocialMediaApp.Api.Models;

public class Like
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Which post was liked
    public int PostId { get; set; }
    public Post? Post { get; set; }

    // Who liked it
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}