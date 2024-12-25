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
	public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        private readonly TagService _tagService;

        public RestaurantController(RestaurantService restaurantService, TagService tagService)
        {
            _restaurantService = restaurantService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _restaurantService.GetAllRestaurants());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _restaurantService.FindRestaurantByIdWithTags(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }
        

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _restaurantService.FindRestaurantById(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            ViewData["Tags"] = await _tagService.GetAllTags();
            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,IsOpen,Description")] Restaurant restaurant,
            long[] TagList)
        {
            if (id != restaurant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    restaurant.Tags = await _tagService.GetTagsByRestaurantId(restaurant.Id);
					await _restaurantService.UpdateRestaurant(restaurant,TagList);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_restaurantService.RestaurantExists(restaurant.Id))
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
            return View(restaurant);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _restaurantService.FindRestaurantById(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var restaurant = await _restaurantService.FindRestaurantById(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            await _restaurantService.DeleteRestaurant(restaurant);
            return RedirectToAction(nameof(Index));
        }
    }
}
