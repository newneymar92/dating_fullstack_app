using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

// Phải là static class
// Static class không thể khởi tạo (không cần new), phù hợp với cách gọi trực tiếp qua tên class.
// Static class chỉ chứa static members, đảm bảo không có state, phù hợp với mục đích utility/helper.
public static class AppUserExtensions
{
    // Phương thức phải là static
    // Tham số đầu tiên có từ khóa "this" - đây là kiểu bạn muốn mở rộng
    // Tham số this là tham số đặc biệt, không cần truyền khi gọi
    public static UserDto ToDto(this AppUser user, ITokenService tokenService)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
    }
} 