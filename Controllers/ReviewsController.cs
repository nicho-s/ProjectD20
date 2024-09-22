using GameForum.Models;
using GameForum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lab4_5.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ForumDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(UserManager<ApplicationUser> userManager, ForumDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return _context.Reviews != null ? 
                          View(await _context.Reviews.ToListAsync()) :
                          Problem("Entity set 'ForumDBContext.Reviews'  is null.");
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("[controller]/Create/{topicId:int}")]
        public async Task<IActionResult> Create(int topicId, NewReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var review = new Review
                {
                    User = user,
                    Text = model.Text,
                    CreatingTime = DateTime.UtcNow,
                    TopicId = topicId // Додаємо цей рядок
                };

                _context.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Topics", new { id = topicId });
            }
            else
            {
                // Вивести в консоль повідомлення про всі помилки валідації
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                // Вивести в Debug-консоль повідомлення про всі помилки валідації
                Debug.WriteLine("ModelState is not valid. Errors:");

                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        Debug.WriteLine($"Key: {key}, Errors: {string.Join(",", state.Errors.Select(x => x.ErrorMessage))}");
                    }
                }
                // Повернути перегляд з помилками валідації
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, UpdReviewViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    review.Text = model.Text;
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Topics");
            }
            return View(review);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reviews == null)
            {
                return Problem("Entity set 'ForumDBContext.Reviews'  is null.");
            }
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        private bool ReviewExists(int id)
        {
          return (_context.Reviews?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> HideRev(HideReviewViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var review = await _context.Reviews.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (review == null) return NotFound();

            if (!review.IsHidden) review.IsHidden = true;

            _context.Reviews.Update(review);

            await _context.SaveChangesAsync();

            return View(model);
        }

        public async Task<IActionResult> UnHideRev(HideReviewViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var review = await _context.Reviews.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (review == null) return NotFound();

            if (review.IsHidden) review.IsHidden = false;

            _context.Reviews.Update(review);

            await _context.SaveChangesAsync();

            return View(model);
        }
    }
}