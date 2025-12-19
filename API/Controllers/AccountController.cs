using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// Inject interface (ITokenService) để code bớt phụ thuộc vào implementation cụ thể, giúp dễ bảo trì, test, và mở rộng.
// AccountController chỉ cần biết: “Có một dịch vụ biết tạo token” → ITokenService.
// Nó không cần biết dịch vụ đó được triển khai như thế nào (TokenService hay một class khác).
// Sau này có thể tạo AnotherTokenService (ví dụ thêm log, rotate key, dùng provider khác) và chỉ cần đổi đăng ký DI trong Program.cs
// DI container làm nhiệm vụ “ghép nối”
// Bạn khai báo interface ở chỗ nhận (constructor), AccountController(AppDbContext context, ITokenService tokenService)
// Bạn cấu hình mapping interface → implementation ở Program.cs.
// Khi chạy, DI container tự động tạo TokenService và đưa vào tham số ITokenService.

// Tóm lại: Inject interface (ITokenService) giúp controller phụ thuộc vào abstraction, không bị “dính chặt” với TokenService, 
// từ đó code linh hoạt, testable, và dễ thay đổi hơn.
public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    // api/account/register
    // Nếu trong Register(string displayname, string email, string password) thì sử dụng [FromBody] để đảm bảo rằng dữ liệu được gửi từ body của request
    // vì nếu không sử dụng [FromBody] thì dữ liệu sẽ được gửi từ query string của request và sẽ không được gửi từ body của request
    // hoặc sử dụng  DTO để đảm bảo rằng dữ liệu được gửi từ body của request
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await EmailExists(registerDto.Email)) return BadRequest("Email taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Sử dụng extension method
        return user.ToDto(tokenService);
        // Tương đương với: return AppUserExtensions.ToDto(user, tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);

        if (user == null) return Unauthorized("Invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (var i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return user.ToDto(tokenService);
    }

    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
    }
}