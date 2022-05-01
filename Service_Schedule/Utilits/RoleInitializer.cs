using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Service_Schedule.Models;

namespace Service_Schedule.Utilits
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string password = "admin123";
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("spec") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("spec"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }
            if (await roleManager.FindByNameAsync("employee") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("employee"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    DateCreate = DateTime.UtcNow.AddHours(3),
                    Email = adminEmail,
                    Name = "Admin",
                    Gender = true,
                    PhoneNumber = "89892328235",
                    UserName = adminEmail,
                    BirthDate = DateTime.Parse("25.06.2001", new CultureInfo("ru-Ru"))
                };
                User Test = new User
                {
                    DateCreate = DateTime.UtcNow.AddHours(3),
                    Email = "test@test.com",
                    Name = "Test",
                    Gender = true,
                    PhoneNumber = "89005553535",
                    UserName = "test@test.com",
                    BirthDate = DateTime.Parse("25.06.2001", new CultureInfo("ru-Ru"))
                };
                IdentityResult resultAdmin = await userManager.CreateAsync(admin, password);
                IdentityResult resultTest = await userManager.CreateAsync(Test, "123123");
                if (resultAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
                if (resultAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(Test, "user");
                }
            }
        }
    }
}
