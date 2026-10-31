using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/posts/{postId}")]
[Authorize]
public class InteractionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public InteractionsController(AppDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // POST api/posts/5/like  (toggle: like if not liked, unlike if liked)
    [HttpPost("like")]
    public async Task<IActionResult> ToggleLike(int postId)
    {
        if (!await _db.Posts.AnyAsync(p => p.Id == postId)) return NotFound();

        var like = await _db.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == CurrentUserId);

        bool liked;
        if (like == null)
        {
            _db.Likes.Add(new Like { PostId = postId, UserId = CurrentUserId });
            liked = true;
        }
        else
        {
            _db.Likes.Remove(like);
            liked = false;
        }

        await _db.SaveChangesAsync();
        var count = await _db.Likes.CountAsync(l => l.PostId == postId);
        return Ok(new { liked, likeCount = count });
    }

    // GET api/posts/5/comments
    [HttpGet("comments")]
    public async Task<IActionResult> GetComments(int postId)
    {
        var comments = await _db.Comments
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                AuthorName = c.User!.DisplayName
            })
            .ToListAsync();

        return Ok(comments);
    }

    // POST api/posts/5/comments
    [HttpPost("comments")]
    public async Task<IActionResult> AddComment(int postId, CreateCommentDto dto)
    {
        if (!await _db.Posts.AnyAsync(p => p.Id == postId)) return NotFound();

        var comment = new Comment
        {
            PostId = postId,
            UserId = CurrentUserId,
            Content = dto.Content
        };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var author = await _db.Users.FindAsync(CurrentUserId);
        return Ok(new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UserId = comment.UserId,
            AuthorName = author!.DisplayName
        });
    }
}