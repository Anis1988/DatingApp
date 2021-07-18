using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
      [HttpPost("register")]
      public async Task<ActionResult<UserDto>> Register(RegisterDto regiserDto)
      {
          if (await UserExists(regiserDto.Username)) return new BadRequestObjectResult("Username already exists");
          using var hmac = new HMACSHA256();

          var user = new AppUser
          {
              UserName = regiserDto.Username.ToLower(),
              PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regiserDto.Password)),
              PasswordSalt = hmac.Key
          };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
      }



      [HttpPost("login")]
      public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
      {
          var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
          if (user == null) return new UnauthorizedObjectResult("Username is incorrect");
          using var hmac = new HMACSHA256(user.PasswordSalt);
          var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
          for (int i = 0; i < computedhash.Length; i++)
          {
              if (computedhash[i] != user.PasswordHash[i]) return new UnauthorizedObjectResult("Password is incorrect");
          }
         return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };;
      }
      private async Task<bool> UserExists(string username)
      {
          return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
      }
    }
}
