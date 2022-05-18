using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]// authorize any call which calls any method of this controller
    public class UsersController : BaseApiController//here we will access properties from baseapicontroller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository,IMapper mapper,
            IPhotoService photoService) // injecting repository,instead od DbConetxt
        {
            _photoService = photoService;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        /*[HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetUsers()
        {
            var users = _context.Users.ToList();
            return users;
        }*/
        //make it asynchronous so that, when next request comes it should not for it
        [HttpGet]
        //[AllowAnonymous] //allow any call
        /* public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() //replace Appuser with MemberDto
         {
             //  var users = await _userRepository.GetUsersAsync();
             //var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);//maps the users to IEnumerable<MemberDto>
             // return Ok(await _userRepository.GetUsersAsync()); //calling repo methods

             //  return Ok(usersToReturn);

             var users = await _userRepository.GetMembersAsync();

             return Ok(users);
         }*/
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages);

            return Ok(users);
        }


        /*[HttpGet("{id}")]
        public ActionResult<AppUser> Getuser(int id)
        {
            var user = _context.Users.Find(id);
            return user;
        }*/
        //[HttpGet("{username}")]
        //[Authorize]//need authorization,for that Microsoft.AspNetCore.Authentication.jwtbearer

        [HttpGet("{username}", Name = "GetUser")]//add name to action ,so that this can be used in parameter of CreatedAtRoute()
        public async Task<ActionResult<MemberDto>> Getuser(string username)
        {
            //var user =  await _userRepository.GetUserByUsernameAsync(username);

            //return _mapper.Map<MemberDto>(user);

            //we are using project in repository , so we no need to convert to mapping we can use directly memberdto
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //we dont have username from UI, so we take it from token
            //we have acces to claims principle of user
            //var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var user = await _userRepository.GetUserByUsernameAsync(username);

            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());//using extension method

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();//save successfully

            return BadRequest("Failed to update user");
        }
         [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new {username = user.UserName} ,_mapper.Map<PhotoDto>(photo));//returns how to get photo by url,and also return data object,
                //username is name of route parameter used with GetUser
            }


            return BadRequest("Problem addding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);//delete from cloudinary
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);//delete from database

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}
