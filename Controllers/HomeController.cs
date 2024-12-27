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
		private readonly CategoryService _categoryService;
		private readonly TagService _tagService;

		public HomeController(ILogger<HomeController> logger, RestaurantService restaurantService, CategoryService categoryService, TagService tagService)
		{
			_logger = logger;
			_restaurantService = restaurantService;
			_categoryService = categoryService;
			_tagService = tagService;
		}

		public async Task<IActionResult> Index()
        {
	        ViewBag.Tags = await _tagService.GetAllTags();
            return View(await _restaurantService.GetAllRestaurantsWithTags());
        }

		public async Task<IActionResult> Shop(Guid id)
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
