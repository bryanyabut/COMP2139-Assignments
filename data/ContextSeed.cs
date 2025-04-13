using Inventory_Management.Areas.OrderManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace Inventory_Management.data;

public class ContextSeed
{
    public static async Task SeedRoles(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(Enum.Roles.SuperAdmin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Enum.Roles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Enum.Roles.Moderator.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Enum.Roles.Basic.ToString()));
    }

    public static async Task SuperSeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var superUser = new ApplicationUser
        {
            UserName = "superadmin",
            Email = "superadmin@example.com",
            FirstName = "super",
            LastName = "admin",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        // Check if the user already exists
        var user = await userManager.FindByEmailAsync(superUser.Email);
        if (user == null)
        {
            // Create the superadmin user
            var result = await userManager.CreateAsync(superUser, "P@ssword12$");
            if (result.Succeeded)
            {
                // Assign roles to the superadmin user
                await userManager.AddToRoleAsync(superUser, Enum.Roles.SuperAdmin.ToString());
                await userManager.AddToRoleAsync(superUser, Enum.Roles.Admin.ToString());
                await userManager.AddToRoleAsync(superUser, Enum.Roles.Moderator.ToString());
                await userManager.AddToRoleAsync(superUser, Enum.Roles.Basic.ToString());
            }
            else
            {
                throw new Exception($"Failed to create superadmin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
    
}