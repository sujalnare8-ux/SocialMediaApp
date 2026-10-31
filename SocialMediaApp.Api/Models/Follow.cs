namespace SocialMediaApp.Api.Models;

public class Follow
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // The user who follows
    public string FollowerId { get; set; } = string.Empty;
    public User? Follower { get; set; }

    // The user being followed
    public string FollowingId { get; set; } = string.Empty;
    public User? Following { get; set; }
}
