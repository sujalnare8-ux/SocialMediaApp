using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PostsController(AppDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET api/posts?page=1&pageSize=10  (everyone)
    [HttpGet]
    public async Task<IActionResult> GetFeed(int page = 1, int pageSize = 10)
    {
        var me = CurrentUserId;

        var posts = await _db.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                AuthorName = p.User!.DisplayName,
                LikeCount = p.Likes.Count,
                CommentCount = p.Comments.Count,
                LikedByMe = p.Likes.Any(l => l.UserId == me)
            })
            .ToListAsync();

        return Ok(posts);
    }

    // GET api/posts/following?page=1&pageSize=10  (me + people I follow)
    [HttpGet("following")]
    public async Task<IActionResult> GetFollowingFeed(int page = 1, int pageSize = 10)
    {
        var me = CurrentUserId;

        var followingIds = _db.Follows
            .Where(f => f.FollowerId == me)
            .Select(f => f.FollowingId);

        var posts = await _db.Posts
            .Where(p => p.UserId == me || followingIds.Contains(p.UserId))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                AuthorName = p.User!.DisplayName,
                LikeCount = p.Likes.Count,
                CommentCount = p.Comments.Count,
                LikedByMe = p.Likes.Any(l => l.UserId == me)
            })
            .ToListAsync();

        return Ok(posts);
    }

    // POST api/posts
    [HttpPost]
    public async Task<IActionResult> Create(CreatePostDto dto)
    {
        var post = new Post
        {
            Content = dto.Content,
            ImageUrl = dto.ImageUrl,
            UserId = CurrentUserId
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        var author = await _db.Users.FindAsync(CurrentUserId);

        return Ok(new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            UserId = post.UserId,
            AuthorName = author!.DisplayName,
            LikeCount = 0,
            CommentCount = 0
        });
    }

    // DELETE api/posts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null) return NotFound();
        if (post.UserId != CurrentUserId) return Forbid();

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}