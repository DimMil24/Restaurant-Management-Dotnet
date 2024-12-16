using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Models;
using Restaurant_Manager.Services;

namespace Restaurant_Manager.Controllers
{
    [Authorize(Roles = "Owner,Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly IAuthorizationService _authorizationService;

        public CategoryController(CategoryService categoryService, IAuthorizationService authorizationService)
        {
            _categoryService = categoryService;
            _authorizationService = authorizationService;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            Guid restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value!);
            return View(await _categoryService.GetCategoriesCountAsync(restaurantId));
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, category, "RestaurantPolicy");

            if (authorizationResult.Succeeded)
            {
                return View(category);
            }
            else if (User.Identity!.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            if (ModelState.IsValid)
            {
                Guid restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value!);
                await _categoryService.CreateCategoryAsync(restaurantId, name);
                return RedirectToAction(nameof(Index));
            }
            return View(name);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, category, "RestaurantPolicy");

            if (authorizationResult.Succeeded)
            {
                return View(category);
            }
            else if (User.Identity!.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }
            
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, string name)
        {
            if (!_categoryService.CategoryExists(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Guid restaurantId = Guid.Parse(User.FindFirst("RestaurantId")?.Value!);
                    Category? category = await _categoryService.GetCategoryByIdAsync(id); //TODO FIX SECURITY. OTHER USER CAN CHANGE NAMES OF CATEGORIES
                    await _categoryService.UpdateCategoryAsync(category,name);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_categoryService.CategoryExists(id))
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

            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, category, "RestaurantPolicy");

            if (authorizationResult.Succeeded)
            {
                return View(category);
            }
            else if (User.Identity!.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category != null)
            {
                var authorizationResult = await _authorizationService
                                                .AuthorizeAsync(User, category, "RestaurantPolicy");

                if (authorizationResult.Succeeded)
                {
                    await _categoryService.DeleteCategoryAsync(category);
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
