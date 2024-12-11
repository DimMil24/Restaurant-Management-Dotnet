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
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restaurant_Manager.Areas.Identity;
using Restaurant_Manager.Data;
using Restaurant_Manager.Services;

namespace Restaurant_Manager.Areas.Identity.Pages.Account
{
	public class RegisterModelAdmin : PageModel
	{
		private readonly SignInManager<CustomIdentityUser> _signInManager;
		private readonly UserService _UserService;

		public RegisterModelAdmin(
			SignInManager<CustomIdentityUser> signInManager,
			UserService userService)
		{
			_signInManager = signInManager;
			_UserService = userService;
		}
		
		[BindProperty]
		public InputModel Input { get; set; }
		
		public string ReturnUrl { get; set; }
		
		public IList<AuthenticationScheme> ExternalLogins { get; set; }
		
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
				await _UserService.RegisterAdmin(Input.UserName, Input.Password);
				return LocalRedirect(returnUrl);
			}
			// If we got this far, something failed, redisplay form
			return Page();
		}
		
	}
}
