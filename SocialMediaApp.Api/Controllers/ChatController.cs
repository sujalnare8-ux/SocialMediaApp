using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.DTOs;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _db;

    public ChatController(AppDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET api/chat/users -> everyone I can chat with
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var me = CurrentUserId;
        var users = await _db.Users
            .Where(u => u.Id != me)
            .OrderBy(u => u.DisplayName)
            .Select(u => new { u.Id, u.DisplayName })
            .ToListAsync();
        return Ok(users);
    }

    // GET api/chat/{userId} -> my conversation with that user
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetConversation(string userId)
    {
        var me = CurrentUserId;

        var messages = await _db.Messages
            .Where(m => (m.SenderId == me && m.ReceiverId == userId)
                     || (m.SenderId == userId && m.ReceiverId == me))
            .OrderByDescending(m => m.SentAt)
            .Take(100)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId
            })
            .ToListAsync();

        messages.Reverse(); // oldest first
        return Ok(messages);
    }
}