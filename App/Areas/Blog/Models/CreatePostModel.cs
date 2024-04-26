﻿using System.ComponentModel.DataAnnotations;
using App.Models.Blog;

namespace App.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }


        [DataType(DataType.Upload)]
        [Display(Name = "Chọn file upload")]
        public IFormFile? FileUpload { get; set; }

    }
}