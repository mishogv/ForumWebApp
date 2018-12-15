namespace Forum.Controllers
{
    using Forum.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class HomeController : Controller
    {
        private readonly ForumDbContext context;

        public HomeController(ForumDbContext db)
        {
            this.context = db;
        }

        public IActionResult Index()
        {
            var categories = context.Categories
                .Include(x => x.Author)
                .Include(x => x.Topics)
                .ThenInclude(x => x.Author).ToList();

            var topics = this.context
                .Topics
                .Include(t => t.Author)
                .Include(t => t.Comments)
                .ThenInclude(x => x.Author)
                .OrderByDescending(t => t.CreatedDate)
                .ThenByDescending(t => t.LastUpdatedDate)
                .ToList();

            ViewData["Categories"] = categories;

            return View(topics);
        }
    }
}
