namespace SocialMediaApp.Api.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    // Who sent the message
    public string SenderId { get; set; } = string.Empty;
    public User? Sender { get; set; }

    // Who receives it
    public string ReceiverId { get; set; } = string.Empty;
    public User? Receiver { get; set; }
}