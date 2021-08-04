
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager /* DataContext context */,
        RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if (users == null) return;
            var roles  = new List<AppRole>
            {
                new AppRole{Name= "Member"},
                new AppRole{Name= "Moderator"},
                new AppRole{Name= "Admin"}
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            };
            foreach (var user in users)
            {
                // using var hmac= new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                // user.PasswordSalt = hmac.Key;
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                // await context.Users.AddAsync(user);
                await userManager.CreateAsync(user,"Pa$$word1234");
                await userManager.AddToRoleAsync(user,"Member");
            }
            var admin  = new AppUser
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin,"Pa$$word1234");
            await userManager.AddToRolesAsync(admin,new [] {"Admin","Moderator"});
            // await context.SaveChangesAsync();
        }
    }
}
