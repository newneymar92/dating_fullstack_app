using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class MembersController(AppDbContext context) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();

            return members;
        }

        // JWT Bearer authentication đã được cấu hình trong Program.cs  
        // app.UseAuthentication() và app.UseAuthorization() đã được thêm vào pipeline
        // Endpoint GetMember được bảo vệ bằng [Authorize], chỉ người dùng đã đăng nhập mới truy cập được.
        [Authorize]
        [HttpGet("{id}")] // locahost:5001/api/members/bob-id
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await context.Users.FindAsync(id);

            if (member == null) return NotFound();

            return member;
        }
    }
}
