using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService,IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }
      [HttpPost("register")]
      public async Task<ActionResult<UserDto>> Register(RegisterDto regiserDto)
      {
          if (await UserExists(regiserDto.Username)) return new BadRequestObjectResult("Username already exists");
          var user  = _mapper.Map<AppUser>(regiserDto);
          using var hmac = new HMACSHA256();


              user.UserName = regiserDto.Username.ToLower();
              user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regiserDto.Password));
              user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
      }



      [HttpPost("login")]
      public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
      {
          var user = await _context.Users
          .Include(p => p.Photos)
          .SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
          if (user == null) return new UnauthorizedObjectResult("Username is incorrect");
          using var hmac = new HMACSHA256(user.PasswordSalt);
          var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
          for (int i = 0; i < computedhash.Length; i++)
          {
              if (computedhash[i] != user.PasswordHash[i]){
                  System.Console.WriteLine();
              return new UnauthorizedObjectResult("Password is incorrect");
              }

          }
         return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };;
      }
      private async Task<bool> UserExists(string username)
      {
          return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
      }
    }
}
