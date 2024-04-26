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
using System.ComponentModel.DataAnnotations;
using System.IO;
using Org.BouncyCastle.Security;
using App.Migrations;
using Microsoft.CodeAnalysis.Differencing;


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
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs,FileUpload")] CreatePostModel post)
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
            if (post.FileUpload == null)
            {
                ModelState.AddModelError("FileUpload", "Ảnh thumbnail không được để trống");
                return View(post);
            }
            else
            {
                var path = Path.Combine("Uploads", "Post_Thumbnail");

                //var path = "~/Uploads/Post_Thumbnail/";
                foreach (var filename in Directory.GetFiles(path)){
                    
                    if (Path.Combine("Uploads", "Post_Thumbnail", post.FileUpload.FileName )== filename)
                    {
                        
                        ModelState.AddModelError("FileUpload", "Tên ảnh bị trùng, vui lòng đổi tên khác");
                        return View(post);

                    }

                }
            }
            
    
            //ModelState.Remove("FileUpload");

            if (ModelState.IsValid) 
            {
                // Thêm ảnh vào folder
                var file = Path.Combine("Uploads", "Post_Thumbnail", post.FileUpload.FileName);
                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await post.FileUpload.CopyToAsync(filestream);// coppy dữ liệu của fileUploads qua filestream
                }
                post.Thumbnail = post.FileUpload.FileName;



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
                CategoryIDs = post.PostCategories.Select(pc => pc.CategoryID).ToArray(),
                Thumbnail = post.Thumbnail
            };
            var categories = await _context.Categories.ToListAsync();

            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");


            return View(postEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,Description,Slug,Published,CategoryIDs,FileUpload")] CreatePostModel post)
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

            if (post.FileUpload != null)
            {

                var path = Path.Combine("Uploads", "Post_Thumbnail");
                foreach (var filename in Directory.GetFiles(path))
                {
                    if (Path.Combine("Uploads", "Post_Thumbnail", post.FileUpload.FileName) == filename)
                    {
                        ModelState.AddModelError("FileUpload", "Tên ảnh bị trùng, vui lòng đổi tên khác");
                        StatusMessage = "Cập nhập hình không thành công";
                        return View(post);

                    }

                }

            }
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var postUpdate = await _context.Posts.Include(p => p.PostCategories)
                        .FirstOrDefaultAsync(p => p.PostId == id);
                    if (postUpdate == null) { return NotFound(); }

                    //Xóa tên file trong thư mục
                    if (post.FileUpload != null)
                    {
                        string filePathToDelete = Path.Combine("Uploads", "Post_Thumbnail", postUpdate.Thumbnail);
                        if (System.IO.File.Exists(filePathToDelete))
                        {
                            System.IO.File.Delete(filePathToDelete);
                            Console.WriteLine("Đã xóa tệp thành công.");
                        }
                        else
                        {
                            Console.WriteLine("Tệp không tồn tại.");
                        }

                        var file1 = Path.Combine("Uploads", "Post_Thumbnail", post.FileUpload.FileName);
                        using (var filestream = new FileStream(file1, FileMode.Create))
                        {
                            await post.FileUpload.CopyToAsync(filestream);
                        }
                        postUpdate.Thumbnail = post.FileUpload.FileName;

                        



                    }


                    // update
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


            //StatusMessage = "Không hiểu lỗi gì";

            return RedirectToAction(nameof(Index));
        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }



        //// Cái này làm ra để xóa hình trong file bị thừa do đợt đầu thêm nhiều quá, không khớp với database + Thêm ảnh cho những thằng post chưa có hình
        //[HttpPost]
        //public async Task<IActionResult> deletePicture()
        //{
        //    var posts = _context.Posts.ToList();
        //    var path = Path.Combine("Uploads", "Post_Thumbnail");
        //    foreach (var filename in Directory.GetFiles(path))
        //    {
        //        bool check = false;
        //        foreach (var post in posts)
        //        {
        //            //if(post.Thumbnail == null)
        //            //{
        //            //        post.Thumbnail = "0419-Cover.jpg";
        //            //        _context.Update(post);
        //            //        await _context.SaveChangesAsync();

        //            //}
        //            if (Path.Combine("Uploads", "Post_Thumbnail", post.Thumbnail) == filename)
        //            {
        //                check = true;
        //                break;
        //            }
        //        }
        //        if(check == false)
        //        {
        //            System.IO.File.Delete(filename);
        //            Console.WriteLine("Đã xóa tệp không tồn tại thành công.");
        //        }                   
        //    }

        //    StatusMessage = " Đã xóa hình không liên quan";
        //    return RedirectToAction(nameof(Index));



        //    // Code thực hiện action trên: lưu ý là phải cùng area và controller nhennn
        //    //< form asp - action = "deletePicture" >
        //    //    < div class="col">
        //    //        <input type = "submit" value="   OK   " class="btn btn-primary" />
        //    //    </div>
        //    //</form>


        //}


}
}


