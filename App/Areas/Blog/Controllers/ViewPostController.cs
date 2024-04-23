using App.Models;
using App.Models.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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

        [Route("/post/{postslug}/")]
        [HttpGet]
        public IActionResult Details(string postslug)
        {
            return View();
        }



    }
}
