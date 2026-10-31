using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Api.DTOs;

public class CreatePostDto
{
    [Required, MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool LikedByMe { get; set; }
}