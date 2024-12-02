// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restaurant_Manager.Areas.Identity;
using Restaurant_Manager.Data;

namespace Restaurant_Manager.Areas.Identity.Pages.Account
{
	public class RegisterModelAdmin : PageModel
	{
		private readonly SignInManager<CustomIdentityUser> _signInManager;
		private readonly UserManager<CustomIdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IUserStore<CustomIdentityUser> _userStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;
		private readonly ApplicationDbContext _context;

		public RegisterModelAdmin(
			UserManager<CustomIdentityUser> userManager,
			IUserStore<CustomIdentityUser> userStore,
			SignInManager<CustomIdentityUser> signInManager,
			ILogger<RegisterModel> logger,
			RoleManager<IdentityRole> roleManager,
			IEmailSender emailSender,
			ApplicationDbContext context)
		{
			_userManager = userManager;
			_userStore = userStore;
			//_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_roleManager = roleManager;
			_logger = logger;
			_emailSender = emailSender;
			_context = context;
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
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[Display(Name = "Username")]
			public string UserName { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
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
					if (!await _roleManager.RoleExistsAsync("Admin"))
					{
						await _roleManager.CreateAsync(new IdentityRole("Admin"));
					}
					await _userManager.AddToRoleAsync(user, "Admin");
					_logger.LogInformation("User Admin created a new account with password.");

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
