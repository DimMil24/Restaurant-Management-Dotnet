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
	public class OrderController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IAuthorizationService _authorizationService;


		public OrderController(ApplicationDbContext context, IAuthorizationService authorizationService)
		{
			_context = context;
			_authorizationService = authorizationService;
		}
		public async Task<IActionResult> Index()
		{
			var restaurant_id = long.Parse(User.FindFirst("RestaurantId")?.Value);
			return View(await _context.CustomerOrder.Include(e => e.User).Where(e => e.RestaurantId == restaurant_id).ToListAsync());
		}

		public async Task<IActionResult> Details(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerOrder = await _context.CustomerOrder
				.Include(c => c.OrderProducts)
				.ThenInclude(p => p.Product)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (customerOrder == null)
			{
				return NotFound();
			}

			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, customerOrder, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				return View(customerOrder);
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
			var restaurant_id = long.Parse(User.FindFirst("RestaurantId")?.Value);
			var restaurants = _context.Restaurant.ToList();
			ViewData["Products"] = new SelectList(_context.Product.Where(p => p.Available && p.RestaurantId==restaurant_id), "Id", "Name");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CustomerOrder customerOrder, [FromForm] List<long> products,
			[FromForm] List<int> quantity)
		{
			if (ModelState.IsValid)
			{
				using var transaction = _context.Database.BeginTransaction();
				var restaurant_id = long.Parse(User.FindFirst("RestaurantId")?.Value);
				var prods = new List<OrderProduct>();
				for (int i = 0; i < products.Count; i++)
				{
					var prod = await _context.Product.FindAsync(products[i]);
					if (prod != null) prods.Add(new OrderProduct { Quantity = quantity[i], Price = prod.Price, Product = prod, CustomerOrder = customerOrder });
				}
				customerOrder.RestaurantId = restaurant_id;
				customerOrder.OrderProducts = prods;
				_context.Add(customerOrder);
				await _context.SaveChangesAsync();
				transaction.Commit();
				return RedirectToAction(nameof(Index));
			}
			return View();
		}

		public async Task<IActionResult> Edit(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerOrder = await _context.CustomerOrder.Include(c => c.OrderProducts).FirstOrDefaultAsync(p => p.Id == id);
			if (customerOrder == null)
			{
				return NotFound();
			}

			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, customerOrder, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				var restaurant_id = long.Parse(User.FindFirst("RestaurantId")?.Value);
				ViewData["Products"] = await _context.Product.Where(p => p.Available && p.RestaurantId == restaurant_id).ToListAsync();
				return View(customerOrder);
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
		public async Task<IActionResult> Edit(long id, [Bind("Id,Open,RestaurantId")] CustomerOrder customerOrder, [FromForm] List<long> products,
			[FromForm] List<int> quantity)
		{
			if (id != customerOrder.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				using var transaction = _context.Database.BeginTransaction();
				try
				{
					var editOrder = await _context.CustomerOrder.Include(c => c.OrderProducts).FirstOrDefaultAsync(p => p.Id == id);
					var prods = new List<OrderProduct>();
					for (int i = 0; i < products.Count; i++)
					{
						var prod = await _context.Product.FindAsync(products[i]);
						if (prod != null) prods.Add(new OrderProduct { Quantity = quantity[i], Price = prod.Price, Product = prod, CustomerOrder = customerOrder });
					}
					editOrder.Open = customerOrder.Open;
					editOrder.OrderProducts = prods;
					_context.Update(editOrder);
					await _context.SaveChangesAsync();
					transaction.Commit();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CustomerOrderExists(customerOrder.Id))
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
			return View(customerOrder);
		}

		public async Task<IActionResult> Delete(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var customerOrder = await _context.CustomerOrder
				.FirstOrDefaultAsync(m => m.Id == id);
			if (customerOrder == null)
			{
				return NotFound();
			}

			var authorizationResult = await _authorizationService
		  .AuthorizeAsync(User, customerOrder, "RestaurantPolicy");

			if (authorizationResult.Succeeded)
			{
				return View(customerOrder);
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
			var customerOrder = await _context.CustomerOrder.FindAsync(id);
			if (customerOrder != null)
			{
				_context.CustomerOrder.Remove(customerOrder);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool CustomerOrderExists(long id)
		{
			return _context.CustomerOrder.Any(e => e.Id == id);
		}

		public async Task<List<CustomerOrder>> Test()
		{
			return await _context.CustomerOrder.ToListAsync();
		}

		[HttpPost]
		public async Task<IActionResult> Complete(long? id)
		{
			var customerOrder = await _context.CustomerOrder.FindAsync(id);
			if (customerOrder is null)
			{
				return NotFound();
			}
			if (!customerOrder.Open)
			{
				return BadRequest();
			} 
			else
			{
				customerOrder.Open = false;
				_context.Update(customerOrder);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
		}
	}
}
