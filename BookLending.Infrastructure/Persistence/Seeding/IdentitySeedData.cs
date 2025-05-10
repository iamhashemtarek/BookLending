using BookLending.Common.Constants;
using BookLending.Domain.Entities;
using BookLending.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Infrastructure.Persistence.Seeding
{
    public static class IdentitySeedData
    {
        public static async Task Initialize(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            IOptions<SystemAdminCredentialsConfig> adminConfig
            )
        {
            string[] roles = { AppRoles.Admin, AppRoles.Member };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await SeedAdminUser(
                userManager,
                adminConfig.Value.Email,
                adminConfig.Value.Password
                );

        }

        private static async Task SeedAdminUser(
            UserManager<AppUser> userManager,
            string email,
            string password
            )
        {
            var adminUser = new AppUser
            {
                UserName = email.Split('@')[0],
                Email = email,
            };
            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
            }
        }
    }
}
