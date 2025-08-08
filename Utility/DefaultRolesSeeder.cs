//using Microsoft.AspNetCore.Identity;

//namespace IEEE.Utility
//{
//    public static class DefaultRolesSeeder
//    {
//        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
//        {
//            string[] roleNames = { "HighBoard", "Head", "Member" };

//            foreach (var roleName in roleNames)
//            {
//                if (!await roleManager.RoleExistsAsync(roleName))
//                {
//                    await roleManager.CreateAsync(new IdentityRole(roleName));
//                }
//            }
//        }
//    }
//}
