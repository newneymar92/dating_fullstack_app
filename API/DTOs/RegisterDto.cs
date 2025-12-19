using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
  // Sử dụng [Required] thay vì public required string DisplayName { get; set; } = ""; để đảm bảo rằng DisplayName không được để trống
  [Required]
  public string DisplayName { get; set; } = "";

  // Sử dụng [Required] và [EmailAddress] để đảm bảo rằng Email không được để trống và là địa chỉ email hợp lệ
  [Required]
  [EmailAddress]
  public string Email { get; set; } = "";

  // Sử dụng [Required] và [MinLength(4)] để đảm bảo rằng Password không được để trống và có ít nhất 4 ký tự
  [Required]
  [MinLength(4)]
  public string Password { get; set; } = "";
}