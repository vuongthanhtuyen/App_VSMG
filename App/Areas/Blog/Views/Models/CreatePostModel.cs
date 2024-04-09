using App.Models.Blog;
using System.ComponentModel.DataAnnotations;

namespace App.Areas.Blog.Views.Models
{
    public class CreatePostModel: Post
    {
        [Display(Name ="Chuyên mục")]
        public int[] CategoryIds { get; set;  }
    }
}
