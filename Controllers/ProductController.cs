using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Services;

namespace Restaurant_Manager.Controllers
{
	[Authorize(Roles = "Owner,Admin")]
	public class ProductController : Controller
	{
		private readonly ProductService _productService;
		private readonly IAuthorizationService _authorizationService;

		public ProductController(ProductService productService, IAuthorizationService authorizationService)
		{
			_productService = productService;
			_authorizationService = authorizationService;
		}

		public async Task<IActionResult> Index()
		{
			var value = User.FindFirst("RestaurantId")?.Value;
			if (value != null)
			{
				var restaurantId = long.Parse(value);
				return View(await _productService.GetRestaurantProducts(restaurantId));
			}

			return View();
		}

		public async Task<IActionResult> Details(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _productService.FindProduct(id);


			if (product == null)
			{
				return NotFound();
			}
			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, product, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				return View(product);
			}
			else if (User.Identity!.IsAuthenticated)
			{
				return new ForbidResult();
			}
			else
			{
				return new ChallengeResult();
			}
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name,Price,Category,Description")] Product product)
		{
			if (ModelState.IsValid)
			{
				long restaurantId = long.Parse(User.FindFirst("RestaurantId")?.Value!);
				await _productService.CreateProduct(product, restaurantId);
				return RedirectToAction(nameof(Index));
			}
			return View(product);
		}

		public async Task<IActionResult> Edit(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _productService.FindProduct(id);
			if (product == null)
			{
				return NotFound();
			}

			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, product, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				return View(product);
			}
			else if (User.Identity!.IsAuthenticated)
			{
				return new ForbidResult();
			}
			else
			{
				return new ChallengeResult();
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Price,Category,Available,Description")] Product product)
		{
			if (id != product.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					await _productService.UpdateProduct(product);
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_productService.ProductExists(product.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(product);
		}

		public async Task<IActionResult> Delete(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _productService.FindProduct(id);
			if (product == null)
			{
				return NotFound();
			}

			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, product, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				return View(product);
			}
			else if (User.Identity!.IsAuthenticated)
			{
				return new ForbidResult();
			}
			else
			{
				return new ChallengeResult();
			}
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(long id)
		{
			var product = await _productService.FindProduct(id);
			
			if (product != null)
			{
				var authorizationResult = await _authorizationService
												.AuthorizeAsync(User, product, "RestaurantPolicy");

				if (authorizationResult.Succeeded)
				{
					await _productService.DeleteProduct((product));
				}
				else if (User.Identity!.IsAuthenticated)
				{
					return new ForbidResult();
				}
				else
				{
					return new ChallengeResult();
				}
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
