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

namespace Restaurant_Manager.Controllers
{
	[Authorize(Roles = "Owner,Admin")]
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IAuthorizationService _authorizationService;

		public ProductController(ApplicationDbContext context, IAuthorizationService authorizationService)
		{
			_context = context;
			_authorizationService = authorizationService;
		}

		public async Task<IActionResult> Index()
		{
			var restaurant_id = long.Parse(User.FindFirst("RestaurantId")?.Value);
			return View(await _context.Product.Where(e => e.RestaurantId == restaurant_id).ToListAsync());
		}

		public async Task<IActionResult> Details(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.Product.FindAsync(id);


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
			else if (User.Identity.IsAuthenticated)
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
				using var transaction = _context.Database.BeginTransaction();
				var restaurant_id = long.Parse(User.FindFirst("RestaurantId")?.Value);
				product.RestaurantId = restaurant_id;
				_context.Add(product);
				await _context.SaveChangesAsync();
				transaction.Commit();
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

			var product = await _context.Product.FindAsync(id);
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
			else if (User.Identity.IsAuthenticated)
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
		public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Price,Category,Available,Description,RestaurantId")] Product product)
		{
			if (id != product.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					using var transaction = _context.Database.BeginTransaction();
					_context.Update(product);
					await _context.SaveChangesAsync();
					transaction.Commit();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductExists(product.Id))
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

			var product = await _context.Product.FindAsync(id);
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
			else if (User.Identity.IsAuthenticated)
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
			var product = await _context.Product.FindAsync(id);



			if (product != null)
			{
				var authorizationResult = await _authorizationService
												.AuthorizeAsync(User, product, "RestaurantPolicy");

				if (authorizationResult.Succeeded)
				{
					_context.Product.Remove(product);
					await _context.SaveChangesAsync();
				}
				else if (User.Identity.IsAuthenticated)
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

		private bool ProductExists(long id)
		{
			return _context.Product.Any(e => e.Id == id);
		}
	}
}
