using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace API.Controllers
{
  
    public class AccountController : BaseApiController
    {
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            Context = context;
            _tokenService = tokenService;
        }

        public DataContext Context { get; }
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        [HttpPost("register")]
        //public async Task<ActionResult<AppUser>> Register(string username, string password)//if [ApiController] is ther then no need of  [FromQuery],it will map automatically with property name given
        //if data cmg from [FromBody],then we have to create DTO to map the incoming data
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username is used");
            }
            var user = _mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;


            Context.Users.Add(user);
            await Context.SaveChangesAsync();
            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };

        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await Context.Users
                 .Include(p => p.Photos) //to include potos which are not there in users
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if(user == null)
            {
                return BadRequest("Invalid username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }
        
        private async Task<bool> UserExists(string username)
        {
            return await Context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}
