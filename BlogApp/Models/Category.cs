using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models

{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }

    }
}