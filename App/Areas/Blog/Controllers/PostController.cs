using App.Data;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using App.Models.Blog;
using App.Areas.Blog.Views.Models;
using App.Utilities;



namespace App.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles =RoleName.Administrator + "," + RoleName.Editor)]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManaget;

        public PostController(AppDbContext context, UserManager<AppUser> userManaget)
        {
            _context = context;
            _userManaget = userManaget;
        }

        public async Task<IActionResult> Index()
        {
            return View(  _context.Posts.ToList()) ;
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            //var categories = await _context.Categories.ToListAsync();

            //ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View();
        }
    }
}
