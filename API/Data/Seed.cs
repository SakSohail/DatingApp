using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context) //this will called from Program.cs - Main method
        {
            if (await context.Users.AnyAsync()) return; //if we have any users in db then return

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");//read data from json file into string 
            //System.Text.Json; inbuilt in .net , earlier  we have to install NewtonSoft.json nuget package
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);//deserilaze string into json object
            if (users == null) return;
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));//same password for all users

                await context.Users.AddAsync(user);
            }

            await context.SaveChangesAsync();
        }
    }
}