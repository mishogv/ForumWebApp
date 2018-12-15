namespace Forum.Controllers
{
    using Forum.Data;
    using Forum.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;

    public class TopicController : Controller
    {
        private readonly ForumDbContext context;

        public TopicController(ForumDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var topics = context.Topics
                .Include(x => x.Author)
                .Include(t => t.Comments)
                .ThenInclude(p => p.Author)
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.LastUpdatedDate)
                .ToList();

            return View(topics);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var topic = this.context
                .Topics
                .Include(t => t.Author)
                .Include(x => x.Category)
                .Include(t => t.Comments)
                .ThenInclude(x => x.Author)
                .ToList()
                .Find(t => t.Id == id);

            if (topic == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(topic);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            var categoryNames = context.Categories.Select(x => x.Name).ToList();

            ViewData["CategoryNames"] = categoryNames;

            TempData["Topic"] = "created topic";
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(string categoryName, Topic topic)
        {
            if (ModelState.IsValid)
            {
                var userId = this.context
                    .Users
                    .Where(x => x.UserName == User.Identity.Name)
                    .FirstOrDefault()
                    .Id;

                topic.CreatedDate = DateTime.Now;
                topic.LastUpdatedDate = DateTime.Now;

                topic.AuthorId = userId;

                if (!context.Categories.Any(c => c.Name == categoryName))
                {
                    return View(topic);
                }

                var categoryId = context.Categories.FirstOrDefault(x => x.Name == categoryName).Id;

                topic.CategoryId = categoryId;

                this.context.Add(topic);
                this.context.SaveChanges();
                TempData["Success"] = $"{User.Identity.Name} successfully add new topic.";
                return RedirectToAction("Index", "Home");
            }
            return View(topic);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var topic = this.context
                .Topics
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Id == id)
                .FirstOrDefault(x => x.Id == id);

            if (topic == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (topic.Id != id)
            {
                return Forbid();
            }

            var categoryNames = context.Categories.Select(x => x.Name).ToList();

            ViewData["CategoryNames"] = categoryNames;

            return View(topic);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(string categoryName, Topic topic)
        {
            if (ModelState.IsValid)
            {
                var topicFromDb = this.context
                    .Topics
                    .Include(x => x.Author)
                    .FirstOrDefault(x => x.Id == topic.Id);

                if (topicFromDb == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (topic.Id != topicFromDb.Id)
                {
                    return Forbid();
                }

                TempData["Success"] = $"{User.Identity.Name} successfully edit {topicFromDb.Title} to {topic.Title}.";

                topicFromDb.LastUpdatedDate = DateTime.Now;
                topicFromDb.Title = topic.Title;
                topicFromDb.Description = topic.Description;

                var categoryId = context.Categories.FirstOrDefault(x => x.Name == categoryName).Id;
                topicFromDb.CategoryId = categoryId;

                this.context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(topic);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var topic = context
                .Topics
                .Include(x => x.Author)
                .FirstOrDefault(x => x.Id == id);

            if (topic == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (topic.Id != id)
            {
                return Forbid();
            }

            return View(topic);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Delete(Topic topic)
        {
            var topicDb = context
                .Topics
                .Include(x => x.Author)
                .FirstOrDefault(x => x.Id == topic.Id);

            if (topicDb == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (topic.Id != topicDb.Id)
            {
                return Forbid();
            }

            var test = User.Identity.Name;

            TempData["Success"] = $"{User.Identity.Name} successfully delete {topicDb.Title}.";

            context.Topics.Remove(topicDb);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}