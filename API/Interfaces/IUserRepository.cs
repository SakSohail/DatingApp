using API.DTOs;
using API.Entities;
using API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);

        //Task<IEnumerable<MemberDto>> GetMembersAsync(); //optimal solution for GetUsersAsync();

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username);//optimal solution for GetUserByUsernameAsync(string username);


    }
}
