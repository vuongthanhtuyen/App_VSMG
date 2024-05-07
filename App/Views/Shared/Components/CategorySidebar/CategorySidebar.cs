using App.Models.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace App.Components
{

    public class CategorySidebar : ViewComponent
    {
        public class CategorySidbarData
        {
            public List<Category> Categories { get; set; }
            public int level { get; set; }
            public string categoryslug { get; set; }
        }
        public IViewComponentResult Invoke( CategorySidbarData data)
        {
            return View(data);
        }

    }
}
