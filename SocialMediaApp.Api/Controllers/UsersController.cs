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
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET api/users/{id}  -> profile with counts
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(string id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        var me = CurrentUserId;

        return Ok(new ProfileDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl,
            FollowerCount = await _db.Follows.CountAsync(f => f.FollowingId == id),
            FollowingCount = await _db.Follows.CountAsync(f => f.FollowerId == id),
            PostCount = await _db.Posts.CountAsync(p => p.UserId == id),
            IsFollowing = await _db.Follows.AnyAsync(f => f.FollowerId == me && f.FollowingId == id),
            IsMe = me == id
        });
    }

    // PUT api/users/me  -> edit my own profile
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var user = await _db.Users.FindAsync(CurrentUserId);
        if (user == null) return NotFound();

        user.DisplayName = dto.DisplayName;
        user.Bio = dto.Bio;
        await _db.SaveChangesAsync();

        return Ok(new { user.DisplayName, user.Bio });
    }

    // POST api/users/{id}/follow  -> toggle follow / unfollow
    [HttpPost("{id}/follow")]
    public async Task<IActionResult> ToggleFollow(string id)
    {
        var me = CurrentUserId;
        if (me == id) return BadRequest("You cannot follow yourself.");
        if (!await _db.Users.AnyAsync(u => u.Id == id)) return NotFound();

        var follow = await _db.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == me && f.FollowingId == id);

        bool following;
        if (follow == null)
        {
            _db.Follows.Add(new Follow { FollowerId = me, FollowingId = id });
            following = true;
        }
        else
        {
            _db.Follows.Remove(follow);
            following = false;
        }

        await _db.SaveChangesAsync();
        var count = await _db.Follows.CountAsync(f => f.FollowingId == id);
        return Ok(new { following, followerCount = count });
    }

    // GET api/users/{id}/posts  -> posts written by this user
    [HttpGet("{id}/posts")]
    public async Task<IActionResult> GetUserPosts(string id)
    {
        var me = CurrentUserId;

        var posts = await _db.Posts
            .Where(p => p.UserId == id)
            .OrderByDescending(p => p.CreatedAt)
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
}