namespace Forum.Controllers
{
    using Forum.Data;
    using Forum.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class CategoryController : Controller
    {
        private readonly ForumDbContext context;

        public CategoryController(ForumDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(Category category)
        {
            var authorId = context
                .Users
                .Where(x => x.UserName == this.User.Identity.Name)
                .First()
                .Id;
            category.AuthorId = authorId;
            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                context.SaveChanges();
                TempData["Success"] = $"{User.Identity.Name} successfully add new category.";
                if (TempData["Topic"] != null)
                {
                    return RedirectToAction("Create", "Topic");
                }
                return RedirectToAction("Index", "Home");
            }

            return View(category);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var category = context
                .Categories
                .Include(x => x.Author)
                .Include(x => x.Topics)
                .ThenInclude(x => x.Comments)
                .ThenInclude(x => x.Author)
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var categories = context
                .Categories
                .Include(x => x.Topics)
                .ToList();
            ViewData["Categories"] = categories;
            return View(category);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var category = context.Categories.Where(x => x.Id == id).FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(category);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var categoryFromDb = context
                    .Categories
                    .SingleOrDefault(x => x.Id.Equals(category.Id));

                if (categoryFromDb == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                TempData["Success"] = $"{User.Identity.Name} successfully edit category {categoryFromDb.Name} to {category.Name}.";

                categoryFromDb.Name = category.Name;

                context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(category);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var category = context
                .Categories
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(category);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var category = context
                .Categories
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"{User.Identity.Name} successfully delete category - {category.Name}.";
            context.Categories.Remove(category);
            context.SaveChanges();

            return RedirectPermanent("/");
        }
    }
}