using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class AccountController(DataContext dbContext, ITokenService tokenService) : BaseApiController
{
  /// <summary>
  /// We can pass the data as query string parameters to send the data
  /// </summary>
  /// <param name="UserName"></param>
  /// <param name="Password"></param>
  /// <returns></returns>
  [HttpPost("Register")]
  public async Task<ActionResult<UserDto>> Register(string UserName, string Password)
  {
    using var hmac = new HMACSHA512();
    var newUser = new AppUser
    {
      UserName = UserName,
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password)),
      PasswordSalt = hmac.Key
    };

    dbContext.Users.Add(newUser);
    await dbContext.SaveChangesAsync();
    return new UserDto
    {
      UserName = newUser.UserName,
      Token = tokenService.CreateToken(newUser)
    };
  }

  /// <summary>
  /// We can pass the data in the body to send the user information
  /// </summary>
  /// <param name="registerDto"></param>
  /// <returns></returns>
  [HttpPost("RegisterUser")]
  public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
  {
    if (await UserExists(registerDto.UserName)) return BadRequest("User already exists");
    using var hmac = new HMACSHA512();
    var newUser = new AppUser
    {
      UserName = registerDto.UserName,
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
      PasswordSalt = hmac.Key
    };

    dbContext.Users.Add(newUser);
    await dbContext.SaveChangesAsync();
    return new UserDto
    {
      UserName = newUser.UserName,
      Token = tokenService.CreateToken(newUser)
    };
  }

  /// <summary>
  /// Login User
  /// </summary>
  /// <param name="loginDto"></param>
  /// <returns></returns>
  [HttpPost("login")]
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
  {
    var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower());

    if (user == null) return Unauthorized("InValid User");

    using var hmac = new HMACSHA512(user.PasswordSalt);

    var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    for (int i = 0; i < computeHash.Length; i++)
    {
      if (computeHash[i] != user.PasswordHash[i])
        return Unauthorized("Invalid Password");
    }

    return new UserDto
    {
      UserName = user.UserName,
      Token = tokenService.CreateToken(user)
    };
  }


  private async Task<bool> UserExists(string userName)
  {
    return await dbContext.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
  }
}