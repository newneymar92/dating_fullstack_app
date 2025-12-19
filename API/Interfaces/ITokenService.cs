using API.Entities;

namespace API.Interfaces;
// Ý tưởng về Interface là chúng ta được tách biệt khỏi việc triển khai cụ thể của một class

// Sử dụng interface chúng ta có thể tạo các service khác nhau, chúng có logic triển khai khác nhau nhưng sử dụng cùng phương pháp
// lấy cùng tham số và trả về cùng kiểu dữ liệu
public interface ITokenService
{
  //  Cách tạo ra token trong CreateToken này không quan trọng, những gì chúng ta biết là khi sử dụng ITokenService, nếu chúng ta sử dụng
  // phương thức CreateToken, chúng ta chỉ cần truyền cho nó một AppUser và nó sẽ trả về một string 
  string CreateToken(AppUser user);
}