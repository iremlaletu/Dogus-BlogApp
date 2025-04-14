using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    public class PostController : Controller
    {
        private readonly MyAppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png" };

        public PostController(MyAppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]
        public IActionResult Index(int? categoryId)

        {
            // Get all posts and include the Category navigation property
            var postsQuery = _context.Posts.Include(p => p.Category).OrderByDescending(p => p.CreatedAt).AsQueryable();


            if (categoryId.HasValue)
            {
                // Filter the posts by the selected category
                postsQuery = postsQuery.Where(p => p.CategoryId == categoryId);
            }
            // If no category is selected, show all posts
            var posts = postsQuery.ToList();
            // This will be used to populate the category filter in the view
            ViewBag.Categories = _context.Categories.ToList();
            return View(posts);
        }

        public async Task<IActionResult> Details(int id)
        {

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }



        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            var postViewModel = new PostViewModel
            {
                Categories = new SelectList(_context.Categories, "Id", "Name"),
            };

            return View(postViewModel);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            postViewModel.Post.Author = User.Identity!.Name!;
            postViewModel.Post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!; // GUID şeklinde bir string

            if (ModelState.IsValid)
            {

                var inputFileExtension = Path.GetExtension(postViewModel.ImageUrl.FileName).ToLower();
                bool isAllowed = _allowedExtension.Contains(inputFileExtension);

                if (!isAllowed)
                {
                    ModelState.AddModelError("ImageUrl", "Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                    return View(postViewModel);
                }


                postViewModel.Post.ImageUrlPath = await UploadFileFolder(postViewModel.ImageUrl);

                await _context.Posts.AddAsync(postViewModel.Post);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");

            }
            // If the model state is not valid, we need to repopulate the categories
            postViewModel.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(postViewModel);
        }

        [Authorize]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            comment.AuthorName = User.Identity!.Name!;
            comment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = comment.PostId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id, string? returnTo)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != currentUserId)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            
            if (returnTo == "dashboard")
                return RedirectToAction("Index", "Dashboard");


            return RedirectToAction("Details", new { id = comment.PostId });
        }


        
        // returns the view to confirm the deletion
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id, string? returnTo)
        {

            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (postFromDb == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner of the post
            if (postFromDb.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

             if (returnTo == "dashboard")
                return RedirectToAction("Index", "Dashboard");

            return View(postFromDb);
        }
 
        // This method is used to delete the post
        // deletes the image file from the server
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (postFromDb == null)
                return NotFound();

            if (postFromDb.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();


            if (!string.IsNullOrEmpty(postFromDb.ImageUrlPath))
            {
                // Delete the image file from the server
                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(postFromDb.ImageUrlPath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }

            }

            _context.Posts.Remove(postFromDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (postFromDb == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (postFromDb.UserId != currentUserId)
            {
                return Forbid();
            }

            // Create a new EditViewModel and populate it with the post data
            EditViewModel editViewModel = new EditViewModel
            {
                Post = postFromDb,
                Categories = new SelectList(_context.Categories, "Id", "Name", postFromDb.CategoryId),
            };

            return View(editViewModel);
        }


        // This method is used to update the post
        // It receives the EditViewModel as a parameter
        // and updates the post in the database
        // It also checks if the image file is valid and uploads it to the server
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }

            var postFromDb = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == editViewModel.Post.Id);

            if (postFromDb == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner of the post
            if (postFromDb.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }


            // set the properties that are not being updated

            editViewModel.Post.Author = postFromDb.Author;
            editViewModel.Post.UserId = postFromDb.UserId;


            // Check if a new image file is uploaded
            if (editViewModel.ImageUrl != null)
            {
                var inputFileExtension = Path.GetExtension(editViewModel.ImageUrl.FileName).ToLower();
                bool isAllowed = _allowedExtension.Contains(inputFileExtension);

                if (!isAllowed)
                {
                    ModelState.AddModelError("ImageUrl", "Invalid file type. Only .jpg, .jpeg, and .png are allowed.");
                    return View(editViewModel);
                }

                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(postFromDb.ImageUrlPath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
                editViewModel.Post.ImageUrlPath = await UploadFileFolder(editViewModel.ImageUrl);
            }
            else
            {
                // If no new image file is uploaded, keep the existing image path
                editViewModel.Post.ImageUrlPath = postFromDb.ImageUrlPath;
            }
            // Update the post properties
            _context.Posts.Update(editViewModel.Post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        // This method is used to upload the image file to the server
        // and return the path to the image file
        // The image file is saved in the wwwroot/images folder
        private async Task<string> UploadFileFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var imagesFolderPath = Path.Combine(wwwRootPath, "images");

            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }

            var filePath = Path.Combine(imagesFolderPath, fileName);

            try
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return "Error: Uploading Images" + ex.Message;
            }

            return "/images/" + fileName;
        }

    }
}