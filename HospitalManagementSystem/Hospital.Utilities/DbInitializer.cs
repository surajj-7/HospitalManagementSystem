using Hospital.Models;
using Hospital.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Utilities
{
    internal class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task Initialize()
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
                throw;
            }

            if (!await _roleManager.RoleExistsAsync(WebSiteRoles.WebSite_Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Admin));
                await _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Patient));
                await _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Doctor));

                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Suraj",
                    Email = "suraj@gmail.com"
                }, "Suraj@123");

                if (result.Succeeded)
                {
                    var appUser = await _userManager.FindByEmailAsync("suraj@gmail.com");
                    if (appUser != null)
                    {
                        await _userManager.AddToRoleAsync(appUser, WebSiteRoles.WebSite_Admin);
                    }
                }
                else
                {
                    Console.WriteLine("Error creating default admin user.");
                }
            }
        }

        async Task IDbInitializer.Initialize()
        {
            await Initialize();
        }
    }
}
