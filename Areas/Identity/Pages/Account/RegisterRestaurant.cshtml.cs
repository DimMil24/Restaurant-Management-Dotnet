// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restaurant_Manager.Areas.Identity;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Areas.Identity.Pages.Account
{
	public class RegisterRestaurantModel : PageModel
	{
		private readonly SignInManager<CustomIdentityUser> _signInManager;
		private readonly UserManager<CustomIdentityUser> _userManager;
		private readonly IUserStore<CustomIdentityUser> _userStore;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;
		//private readonly IUserEmailStore<CustomIdentityUser> _emailStore;
		private readonly ILogger<RegisterRestaurantModel> _logger;
		private readonly IEmailSender _emailSender;

		public RegisterRestaurantModel(
			UserManager<CustomIdentityUser> userManager,
			IUserStore<CustomIdentityUser> userStore,
			SignInManager<CustomIdentityUser> signInManager,
			ILogger<RegisterRestaurantModel> logger,
			ApplicationDbContext applicationDbContext,
			RoleManager<IdentityRole> roleManager,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_userStore = userStore;
			//_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_logger = logger;
			_roleManager = roleManager;
			_context = applicationDbContext;
			_emailSender = emailSender;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
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

			[Required]
			[Display(Name = "Restaurant Name")]
			public string RestaurantName { get; set; }

			[Required]
			[Display(Name = "Restaurant Description")]
			public string RestaurantDescription { get; set; }
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

					var new_restaurant = new Restaurant { Name = Input.RestaurantName, Description = Input.RestaurantDescription, IsOpen = true};
					_context.Add(new_restaurant);
					var added_Restaurant = _context.ChangeTracker.Entries().Where(x => x.State == EntityState.Added).FirstOrDefault();
					await _context.SaveChangesAsync();
					user.RestaurantId = added_Restaurant.CurrentValues.GetValue<long>("Id");
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
					transaction.Commit();
					return LocalRedirect(returnUrl);
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
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

		private IUserEmailStore<CustomIdentityUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<CustomIdentityUser>)_userStore;
		}
	}
}
