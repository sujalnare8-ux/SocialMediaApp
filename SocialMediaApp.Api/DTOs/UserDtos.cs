using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Api.DTOs;

public class ProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsMe { get; set; }
}

public class UpdateProfileDto
{
    [Required, MaxLength(50)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Bio { get; set; }
}

public class MessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
}