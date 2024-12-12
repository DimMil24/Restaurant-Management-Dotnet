using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_Manager.Data;
using Restaurant_Manager.Services;

namespace Restaurant_Manager.Areas.Identity.Pages.Account
{
    public class RegisterUserModel : PageModel
    {
		private readonly SignInManager<CustomIdentityUser> _signInManager;
		private readonly UserService _userService;

		public RegisterUserModel(
			SignInManager<CustomIdentityUser> signInManager, 
			 UserService userService)
		{
			_signInManager = signInManager;
			_userService = userService;
		}

		[BindProperty]
		public InputModel? Input { get; set; }
		
		public string? ReturnUrl { get; set; }
		
		public IList<AuthenticationScheme>? ExternalLogins { get; set; }
		
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
		public async Task OnGetAsync(string? returnUrl = null)
		{
			if (returnUrl != null) ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				await _userService.RegisterUser(Input!.UserName, Input.Password);
				return LocalRedirect(returnUrl);
			}
			return Page();
		}
	}
}
