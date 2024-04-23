using System.ComponentModel.DataAnnotations;
using App.Models.Blog;

namespace App.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }


        [Required(ErrorMessage = "Phải chọn file upload")]
        [DataType(DataType.Upload)]
        [Display(Name = "Chọn file upload")]
        public IFormFile? FileUpload { get; set; }

    }
}