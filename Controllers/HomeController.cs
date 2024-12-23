using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Data;
using Restaurant_Manager.Models;
using Restaurant_Manager.Services;

namespace Restaurant_Manager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly RestaurantService _restaurantService;
		private readonly ProductService _productService;
		private readonly CategoryService _categoryService;

		public HomeController(ILogger<HomeController> logger, RestaurantService restaurantService, ProductService productService, CategoryService categoryService)
		{
			_logger = logger;
			_restaurantService = restaurantService;
			_productService = productService;
			_categoryService = categoryService;
		}

		public async Task<IActionResult> Index()
        {
            return View(await _restaurantService.GetAllRestaurants());
        }

		public async Task<IActionResult> RestaurantPreview(Guid id)
		{
            ViewBag.Id = id;
            ViewBag.Categories = await _categoryService.GetCategoriesByRestaurant(id);
			return View(await _categoryService.GetCategoriesAndProducts(id));
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
