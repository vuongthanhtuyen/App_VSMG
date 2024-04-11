using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Blog;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using App.Areas.Blog.Models;
using Microsoft.AspNetCore.Identity;
using App.Utilities;
using App.Areas.Blog.Models;
using System.Collections.Specialized;

namespace AppMvc.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        // GET: Blog/Post
        public async Task<IActionResult> Index([FromQuery(Name ="p")] int currentPage, int pagesize)
        {
            
            // Lấy ra các bài post có số lượng ngày cập nhập gần nhất
            var posts = _context.Posts
                .Include(p => p.Author)
                .OrderByDescending(p => p.DateUpdated);
            int totalPost = await posts.CountAsync();
            if (pagesize < 10) pagesize =10;
            int countPages = (int)Math.Ceiling((double)totalPost / pagesize);

            if(currentPage> countPages) currentPage = countPages;

            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })
            };



            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPost = totalPost;
            ViewBag.postIndex = (currentPage - 1) * pagesize;


            var postsInPage = await posts.Skip((currentPage - 1) * pagesize)
                .Take(pagesize)
                .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
                .ToListAsync();


            return View(postsInPage);
        }



        // GET: Blog/Post/Create
        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            var categories = await _context.Categories.ToListAsync();

            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            var categories = await _context.Categories.ToListAsync();

            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            if(post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            }
            if(await _context.Posts.AnyAsync(p =>p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Chuỗi url này đã tồn tại, vui lòng nhập lại chuỗi url khác");
                return View(post);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                post.DateCreated = post.DateUpdated = DateTime.Now;
                post.AuthorId = user.Id;
                _context.Add(post);
                if(post.CategoryIDs!= null)
                {
                    foreach(var CateId in post.CategoryIDs)
                    {
                        _context.Add(new PostCategory()
                        {
                            CategoryID = CateId,
                            Post = post
                        });
                    }
                }
                await _context.SaveChangesAsync();
                StatusMessage = "Đã tạo bài viết";
                return RedirectToAction(nameof(Index));
            }
            return View(post);



        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var post = await _context.Posts
                .Include(p=> p.Author)
                .FirstOrDefaultAsync(c => c.PostId == id);

            if (post == null)
            {
                return NotFound();

            }

            return View(post);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id==null)
            {
                return NotFound();

            }
            var post = await _context.Posts.FirstOrDefaultAsync(c=>c.PostId ==id);

            if (post == null)
            {
                return NotFound();

            }

            StatusMessage = "Bạn đã xóa thành công!!";
            _context.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();

            }
            var post = await _context.Posts
                .Include(p => p.PostCategories)
                .FirstOrDefaultAsync(c => c.PostId == id);

            if (post == null)
            {
                return NotFound();

            }

            var postEdit = new CreatePostModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Slug = post.Slug,
                Published = post.Published,
                CategoryIDs = post.PostCategories.Select(pc => pc.CategoryID).ToArray()
            };
            var categories = await _context.Categories.ToListAsync();

            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");


            return View(postEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,Description,Slug,Published,CategoryIDs")] CreatePostModel post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            if (post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            }

            var posts = await _context.Posts.FindAsync(post.PostId);

            if (ModelState.IsValid)
            {
                try
                {
                    var postUpdate = await _context.Posts.Include(p => p.PostCategories)
                        .FirstOrDefaultAsync(p => p.PostId == id);
                    if (postUpdate == null) { return NotFound(); }
                    postUpdate.Title = post.Title;
                    postUpdate.Slug = post.Slug;
                    postUpdate.Description = post.Description;
                    postUpdate.Content = post.Content;
                    postUpdate.DateUpdated = DateTime.Now;
                    postUpdate.Published = post.Published;


                    // Update PostCategory
                    if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };
                    var oldCateIds = postUpdate.PostCategories.Select(c => c.CategoryID).ToArray();
                    var newCateIds = post.CategoryIDs;

                    var removeCatePosts = from postCate in postUpdate.PostCategories
                                          where (!newCateIds.Contains(postCate.CategoryID))
                                          select postCate;

                    _context.PostCategoties.RemoveRange(removeCatePosts);
                    var addCateIds = from CateId in newCateIds
                                     where !oldCateIds.Contains(CateId)
                                     select CateId;
                    foreach(var cateId in addCateIds)
                    {
                        _context.PostCategoties.Add(new PostCategory()
                        {
                            PostID = id,
                            CategoryID = cateId
                        });
                    }


                    _context.Update(postUpdate);
                    await _context.SaveChangesAsync();
                    StatusMessage = "Bạn đã cập nhập thành công";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            //await _context.Update(post);



            return RedirectToAction(nameof(Index));
        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }


    }
}


// Đã làm xong create - done
// Edit - Done
// Delete Done
// Details Done