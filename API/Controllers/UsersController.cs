using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    
    public class UsersController : BaseApiController//here we will access properties from baseapicontroller
    {
        public DataContext _context { get; }
        public UsersController(DataContext context)
        {
            _context = context;
        }
        /*[HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetUsers()
        {
            var users = _context.Users.ToList();
            return users;
        }*/
        //make it asynchronous so that, when next request comes it should not for it
        [HttpGet]
        [AllowAnonymous] //allow any call
        public async Task<IEnumerable<AppUser>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        /*[HttpGet("{id}")]
        public ActionResult<AppUser> Getuser(int id)
        {
            var user = _context.Users.Find(id);
            return user;
        }*/
        [HttpGet("{id}")]
        [Authorize]//need authorization,for that Microsoft.AspNetCore.Authentication.jwtbearer
        public async Task<ActionResult<AppUser>> Getuser(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
