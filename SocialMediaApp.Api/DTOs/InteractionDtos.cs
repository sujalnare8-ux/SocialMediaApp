using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Api.DTOs;

public class CreateCommentDto
{
    [Required, MaxLength(500)]
    public string Content { get; set; } = string.Empty;
}

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
}