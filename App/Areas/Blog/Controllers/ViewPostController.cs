using App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace App.Areas.Blog.Controllers
{
    [Area("Blog")]
    public class ViewPostController : Controller
    {
        private readonly AppDbContext _context;

        public ViewPostController(AppDbContext context)
        {
            _context = context;
        }

        [Route("/post/{categoryslug?}")]
        public async Task<IActionResult> Index()
        {

            var posts = _context.Posts
                .AsQueryable() // Chuyển đổi dữ liệu từ tập hợp Posts thành một đối tượng IQueryable

                .ToList(); 
            return View(posts) ;
        }
    }
}
