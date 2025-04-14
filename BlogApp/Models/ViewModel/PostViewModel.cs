using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Models.ViewModel
{
    public class PostViewModel
    {
        public Post Post { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }

        public IFormFile ImageUrl { get; set; }
    }
}