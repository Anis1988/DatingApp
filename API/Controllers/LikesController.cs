using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        public readonly IUserRepository _userRepository;
        public readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username) // username = zzz.Username
        {
            var sourceUserId = User.GetUserId(); /// zzz.Id
            var likedUser = await _userRepository.GetUserByUsernameAsync(username); // anis
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);  // zzz
            if (likedUser == null) return NotFound(); // Anis not found
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");
            var userLike = await _likesRepository.GetUserLike(sourceUserId,likedUser.Id);
            if (userLike != null) return BadRequest("You Already like this user");
            userLike = new UserLike()
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike); //  ading the "like" to List of liked user
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to like user");

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users =  await _likesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalsPages);
            return Ok(users);
        }
    }
}
