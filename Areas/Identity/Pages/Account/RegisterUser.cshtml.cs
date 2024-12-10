using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_Manager.Data;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Models;
using System.Security.Claims;

namespace Restaurant_Manager.Areas.Identity.Pages.Account
{
    public class RegisterUserModel : PageModel
    {
		private readonly SignInManager<CustomIdentityUser> _signInManager;
		private readonly UserManager<CustomIdentityUser> _userManager;
		private readonly IUserStore<CustomIdentityUser> _userStore;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;
		//private readonly IUserEmailStore<CustomIdentityUser> _emailStore;
		private readonly ILogger<RegisterUserModel> _logger;

		public RegisterUserModel(SignInManager<CustomIdentityUser> signInManager, 
			UserManager<CustomIdentityUser> userManager, 
			IUserStore<CustomIdentityUser> userStore, 
			RoleManager<IdentityRole> roleManager, 
			ApplicationDbContext context,
			ILogger<RegisterUserModel> logger)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_userStore = userStore;
			_roleManager = roleManager;
			_context = context;
			_logger = logger;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
			[Required]
			[Display(Name = "Username")]
			public string UserName { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }

		}
		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				using var transaction = _context.Database.BeginTransaction();
				var user = CreateUser();
				await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
				var result = await _userManager.CreateAsync(user, Input.Password);
				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");
					await _userManager.UpdateAsync(user);
					if (!await _roleManager.RoleExistsAsync("User"))
					{
						await _roleManager.CreateAsync(new IdentityRole("User"));
					}
					await _userManager.AddToRoleAsync(user, "User");
					await _signInManager.SignInAsync(user, isPersistent: false);
					transaction.Commit();
					return LocalRedirect(returnUrl);
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return Page();
		}

		private CustomIdentityUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<CustomIdentityUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(CustomIdentityUser)}'. " +
					$"Ensure that '{nameof(CustomIdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}
	}
}
