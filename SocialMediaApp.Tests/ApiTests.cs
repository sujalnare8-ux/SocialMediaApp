using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.Models;
using SocialMediaApp.Api.Services;

namespace SocialMediaApp.Tests;

public class ApiTests
{
    private static TokenService CreateTokenService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "a-test-key-that-is-at-least-32-characters-long!!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();
        return new TokenService(config);
    }

    [Fact]
    public void CreateToken_ContainsIssuerAndUserEmail()
    {
        var user = new User { Id = "u1", Email = "a@test.com", DisplayName = "Alice" };

        var token = CreateTokenService().CreateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("TestIssuer", jwt.Issuer);
        Assert.Contains(jwt.Claims, c => c.Value == "a@test.com");
    }

    [Fact]
    public void CreateToken_ExpiresInTheFuture()
    {
        var user = new User { Id = "u1", Email = "a@test.com", DisplayName = "Alice" };

        var token = CreateTokenService().CreateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task Database_CountsLikesPerPost()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);
        var post = new Post { Content = "Hello", UserId = "u1" };
        db.Posts.Add(post);
        await db.SaveChangesAsync();

        db.Likes.Add(new Like { PostId = post.Id, UserId = "u1" });
        db.Likes.Add(new Like { PostId = post.Id, UserId = "u2" });
        await db.SaveChangesAsync();

        var count = await db.Likes.CountAsync(l => l.PostId == post.Id);
        Assert.Equal(2, count);
    }
}