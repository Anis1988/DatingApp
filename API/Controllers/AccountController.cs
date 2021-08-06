using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager /*DataContext context*/,
        SignInManager<AppUser> signInManager, ITokenService tokenService,IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
      [HttpPost("register")]
      public async Task<ActionResult<UserDto>> Register(RegisterDto regiserDto)
      {
          if (await UserExists(regiserDto.Username)) return BadRequest("Username already exists");
          var user  = _mapper.Map<AppUser>(regiserDto);
            //  using var hmac = new HMACSHA256();
            //   user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regiserDto.Password));
            //   user.PasswordSalt = hmac.Key;
              user.UserName = regiserDto.Username.ToLower();

            // _context.Users.Add(user);
            // await _context.SaveChangesAsync();
            var result = await _userManager.CreateAsync(user,regiserDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user,"Member");
            if(!roleResult.Succeeded) return BadRequest(result.Errors);
            return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
      }



      [HttpPost("login")]
      public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
      {
          var user = await _userManager.Users
          .Include(p => p.Photos)
          .SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
          if (user == null) return Unauthorized("Username is incorrect");
        //   using var hmac = new HMACSHA256(user.PasswordSalt);
        //   var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        //   for (int i = 0; i < computedhash.Length; i++)
        //   {
        //       if (computedhash[i] != user.PasswordHash[i]){
        //           System.Console.WriteLine();
        //       return Unauthorized("Password is incorrect");
        //       }

        //   }
        var result = await _signInManager.CheckPasswordSignInAsync(user,loginDto.Password,false);
        if(!result.Succeeded) return Unauthorized();
         return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };;
      }
      private async Task<bool> UserExists(string username)
      {
          return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
      }
    }
}
