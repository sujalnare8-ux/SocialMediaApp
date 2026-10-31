using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _db;

    public ChatHub(AppDbContext db)
    {
        _db = db;
    }

    // React calls this method to send a message
    public async Task SendMessage(string receiverId, string content)
    {
        var senderId = Context.UserIdentifier!;

        if (string.IsNullOrWhiteSpace(content) || content.Length > 1000) return;
        if (senderId == receiverId) return;
        if (!await _db.Users.AnyAsync(u => u.Id == receiverId)) return;

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content.Trim()
        };
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        var dto = new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            SentAt = message.SentAt,
            SenderId = senderId,
            ReceiverId = receiverId
        };

        // Push to the receiver instantly, and echo back to the sender
        await Clients.User(receiverId).SendAsync("ReceiveMessage", dto);
        await Clients.User(senderId).SendAsync("ReceiveMessage", dto);
    }
}