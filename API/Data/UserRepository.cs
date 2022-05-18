using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper) //we are using Datacontext , so we need constructor and we need to inject context
        {
            _mapper = mapper;
            _context = context;
        }

        public async  Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
            // return await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            //return await _context.Users.ToListAsync();
            return await _context.Users
                .Include(p => p.Photos) //this will fetch related data(join tables),eager loading
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;//if change saved then it return greator than 0
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;//this will not update ,but it say to EF that it is mark as modifed 
        }

          public async Task<MemberDto> GetMemberAsync(string username)
          {
              return await _context.Users
                  .Where(x => x.UserName == username)
                  .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) //this is automapper ,that projects only specific property provided in dto configuration (ForMember)
                  .SingleOrDefaultAsync();
          }
        

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
          
            var query = _context.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper
                .ConfigurationProvider).AsNoTracking(),
                    userParams.PageNumber, userParams.PageSize);
        }
       /* public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }*/
    }
}
