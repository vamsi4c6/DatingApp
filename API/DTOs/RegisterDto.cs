using System.ComponentModel.DataAnnotations;

namespace API;

public class RegisterDto
{
  [Required]
  [MaxLength(10)]
  public required string UserName { get; set; }
  [Required]
  public required string Password { get; set; }

}