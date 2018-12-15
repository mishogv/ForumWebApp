namespace Forum.Controllers
{
    using Forum.Data;
    using Forum.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;

    public class CommentController : Controller
    {
        private readonly ForumDbContext context;

        public CommentController(ForumDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("/Topic/Details/{id}/Comment/Create")]
        public IActionResult Create(int id)
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("/Topic/Details/{TopicId}/Comment/Create")]
        public IActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreatedDate = DateTime.Now;
                comment.LastUpdatedDate = DateTime.Now;

                string authorId = context
                    .Users
                    .Where(x => x.UserName == User.Identity.Name)
                    .FirstOrDefault()
                    .Id;

                comment.AuthorId = authorId;

                Topic topic = this.context.Topics.Find(comment.TopicId);
                topic.LastUpdatedDate = DateTime.Now;

                context.Comments.Add(comment);

                context.SaveChanges();

                TempData["Success"] = $"{User.Identity.Name} successfully add comment.";

                return Redirect($"/Topic/Details/{comment.TopicId}");
            }
            return View(comment);
        }

        [Authorize]
        [HttpGet]
        [Route("/Topic/Details/{TopicId}/Comment/Edit/{id}")]
        public IActionResult Edit(int? topicId, int? id)
        {
            if (id == null)
            {
                return RedirectPermanent($"/Topic/Details/{topicId}");
            }

            Comment comment = context
                .Comments
                .Include(x => x.Author)
                .Include(x => x.Topic)
                .ThenInclude(t => t.Author)
                .FirstOrDefault(c => c.CommentId == id);

            if (comment == null)
            {
                return RedirectPermanent($"/Topic/Details/{topicId}");
            }
            return View(comment);
        }

        [Authorize]
        [HttpPost]
        [Route("/Topic/Details/{TopicId}/Comment/Edit/{id}")]
        public IActionResult Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                var commentFromDb = context
                    .Comments
                    .Include(c => c.Author)
                    .FirstOrDefault(x => x.CommentId.Equals(comment.CommentId));

                if (commentFromDb == null)
                {
                    return RedirectPermanent($"/Topic/Details/{comment.TopicId}");

                }

                commentFromDb.Description = comment.Description;
                commentFromDb.LastUpdatedDate = DateTime.Now;

                context.SaveChanges();

                TempData["Success"] = $"{User.Identity.Name} successfully edit comment.";

                return Redirect($"/Topic/Details/{comment.TopicId}");
            }
            return View(comment);
        }

        [Authorize]
        [HttpGet]
        [Route("/Topic/Details/{TopicId}/Comment/Delete/{id}")]
        public IActionResult Delete(int topicId, int? id)
        {
            if (id == null)
            {
                return RedirectPermanent($"/Topic/Details/{topicId}");
            }

            var comment = context
                .Comments
                .Include(x => x.Author)
                .Include(x => x.Topic)
                .ThenInclude(t => t.Author)
                .FirstOrDefault(c => c.CommentId == id);

            if (comment == null)
            {
                return RedirectPermanent($"/Topic/Details/{topicId}");
            }

            return View(comment);
        }


        [Authorize]
        [HttpPost]
        [Route("/Topic/Details/{TopicId}/Comment/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var comment = context
                .Comments
                .Find(id);

            if (comment != null)
            {
                var topic = context.Topics.Find(comment.TopicId);
                topic.LastUpdatedDate = DateTime.Now;
                context.Comments.Remove(comment);
                context.SaveChanges();
            }
            TempData["Success"] = $"{User.Identity.Name} successfully delete comment.";

            return RedirectPermanent($"/Topic/Details/{comment.TopicId}");
        }
    }
}