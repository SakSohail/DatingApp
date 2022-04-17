using API.Data;
using API.DTOs;
using API.Entities;
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

        public UsersController(IUserRepository userRepository,IMapper mapper) // injecting repository,instead od DbConetxt
        {
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
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() //replace Appuser with MemberDto
        {
            //  var users = await _userRepository.GetUsersAsync();
            //var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);//maps the users to IEnumerable<MemberDto>
            // return Ok(await _userRepository.GetUsersAsync()); //calling repo methods

            //  return Ok(usersToReturn);

            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }
        /*[HttpGet("{id}")]
        public ActionResult<AppUser> Getuser(int id)
        {
            var user = _context.Users.Find(id);
            return user;
        }*/
        [HttpGet("{username}")]
        //[Authorize]//need authorization,for that Microsoft.AspNetCore.Authentication.jwtbearer
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
            //we dont username from UI, so we take it from token
            //we have acces to claims principle of user
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();//save successfully

            return BadRequest("Failed to update user");
        }
    }
}
