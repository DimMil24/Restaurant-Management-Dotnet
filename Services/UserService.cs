using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Areas.Identity;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<CustomIdentityUser> _signInManager;
    private readonly UserManager<CustomIdentityUser> _userManager;
    private readonly IUserStore<CustomIdentityUser> _userStore;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public UserService(ApplicationDbContext context, 
        RoleManager<IdentityRole> roleManager, 
        IUserStore<CustomIdentityUser> userStore,
        UserManager<CustomIdentityUser> userManager, 
        SignInManager<CustomIdentityUser> signInManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userStore = userStore;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task RegisterUser(String username, String password)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var user = new CustomIdentityUser();
        await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.UpdateAsync(user);
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
        await transaction.CommitAsync();
    }

    public async Task RegisterRestaurantOwner(String username, String password,String restaurantName, String restaurantDescription)
    {
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var user = new CustomIdentityUser();
            await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var newRestaurant = new Restaurant { Name = restaurantName, Description = restaurantDescription, IsOpen = true};
                _context.Add(newRestaurant);
                var addedRestaurant = _context.ChangeTracker.Entries().FirstOrDefault(x => x.State == EntityState.Added);
                await _context.SaveChangesAsync();
                if (addedRestaurant != null) 
                    user.RestaurantId = addedRestaurant.CurrentValues.GetValue<long>("Id");
                await _userManager.UpdateAsync(user);
                var claims = new List<Claim>
                {
                    new Claim("RestaurantId", user.RestaurantId.ToString()), // Add RestaurantId claim
                };

                if (!await _roleManager.RoleExistsAsync("Owner"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Owner"));
                }
                await _userManager.AddToRoleAsync(user, "Owner");
                await _userManager.AddClaimsAsync(user,claims);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            await transaction.CommitAsync();
        }
    }
    
    public async Task RegisterAdmin(String username, String password)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var user = new CustomIdentityUser();
        await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.UpdateAsync(user);
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            await _userManager.AddToRoleAsync(user, "Admin");
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
        await transaction.CommitAsync();
    }
}