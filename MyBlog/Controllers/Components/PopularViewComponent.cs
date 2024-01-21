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
    public class PopularViewComponent : ViewComponent
    {
        private readonly dbMyBlogsContext _context;
        private IMemoryCache _memoryCache;

        public PopularViewComponent(dbMyBlogsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            var _tinseo = _memoryCache.GetOrCreate(CacheKeys.Popular, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsPosts();
            });
            return View(_tinseo);
        }

        public List<Post> GetlsPosts()
        {
            List<Post> lstins = new List<Post>();
            lstins = _context.Posts.AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Views)
                .Take(6)
                .ToList();
            return lstins;
        }
    }
}