using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyBlog.Enums;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using MyBlog.Enums;
using MyBlog.Models;

namespace WebBlog.Controllers.Components
{
    public class NavViewComponent : ViewComponent
    {
        private readonly dbMyBlogsContext _context;
        private IMemoryCache _memoryCache;

        public NavViewComponent(dbMyBlogsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            var _danhmucs = _memoryCache.GetOrCreate(CacheKeys.Categories, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsCategories();
            });
            return View(_danhmucs);
        }

        public List<Category> GetlsCategories()
        {
            List<Category> lstins = new List<Category>();
            lstins = _context.Categories.AsNoTracking()
               .Where(x => x.Published == true)
               .OrderBy(x => x.Ordering)
               .ToList();
            return lstins;
        }
    }
}