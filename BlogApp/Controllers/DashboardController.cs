using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly MyAppDbContext _context;

        public DashboardController(MyAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Category)
                .ToList();

            var comments = _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.Post)
                .ToList();

            ViewBag.MyComments = comments;

            return View(posts);
        }
    }
}
