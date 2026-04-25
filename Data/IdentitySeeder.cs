using IEEE.Entities;
using Microsoft.AspNetCore.Identity;

namespace IEEE.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            UserManager<User> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            // Create Role
            if (!await roleManager.RoleExistsAsync("High Board"))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "High Board"
                });
            }

            // Create User
            var user = await userManager.FindByEmailAsync("highboard@ieee.com");

            if (user == null)
            {
                var newUser = new User
                {
                    UserName = "highboard@ieee.com",
                    Email = "highboard@ieee.com",
                    FName = "High",
                    LName = "Board",
                    MName = "www",
                    IsActive = true,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(newUser, "Admin@123");
                await userManager.AddToRoleAsync(newUser, "High Board");
            }
        }
    }
}