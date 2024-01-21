using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.Helpers;
using MyBlog.Models;
using PagedList.Core;

namespace MyBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly dbMyBlogsContext _context;

        public CategoriesController(dbMyBlogsContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE; //20
            var lsCategories = _context.Categories.AsNoTracking()
                 .OrderByDescending(x => x.CatId);
            PagedList<Category> models = new PagedList<Category>(lsCategories, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["DanhMucGoc"] = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parents,Levels,Icon,Cover,Description")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumb, Microsoft.AspNetCore.Http.IFormFile fCover, Microsoft.AspNetCore.Http.IFormFile fIcon)
        {
            if (ModelState.IsValid)
            {
                category.Alias = Utilities.SEOUrl(category.CatName);
                if (category.Parents == null)
                {
                    category.Levels = 1;
                }
                else
                {
                    category.Levels = category.Parents == 0 ? 1 : 2;
                }
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string Newname = Utilities.SEOUrl(category.CatName) + "preview_" + extension;
                    category.Thumb = await Utilities.UploadFile(fThumb, @"categories\", Newname.ToLower());
                }
                if (fCover != null)
                {
                    string extension = Path.GetExtension(fCover.FileName);
                    string Newname = "cover_" + Utilities.SEOUrl(category.CatName) + extension;
                    category.Cover = await Utilities.UploadFile(fCover, @"covers\", Newname.ToLower());
                }
                if (fIcon != null)
                {
                    string extension = Path.GetExtension(fIcon.FileName);
                    string Newname = "icon_" + Utilities.SEOUrl(category.CatName) + extension;
                    category.Icon = await Utilities.UploadFile(fIcon, @"icons\", Newname.ToLower());
                }

                _context.Add(category);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parents,Levels,Icon,Cover,Description")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumb, Microsoft.AspNetCore.Http.IFormFile fCover, Microsoft.AspNetCore.Http.IFormFile fIcon)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.Alias = Utilities.SEOUrl(category.CatName);
                    if (category.Parents == null)
                    {
                        category.Levels = 1;
                    }
                    else
                    {
                        category.Levels = category.Parents == 0 ? 1 : 2;
                    }
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string Newname = Utilities.SEOUrl(category.CatName) + "preview_" + extension;
                        category.Thumb = await Utilities.UploadFile(fThumb, @"categories \", Newname.ToLower());
                    }
                    if (fCover != null)
                    {
                        string extension = Path.GetExtension(fCover.FileName);
                        string Newname = "cover_" + Utilities.SEOUrl(category.CatName) + extension;
                        category.Cover = await Utilities.UploadFile(fCover, @"covers\", Newname.ToLower());
                    }
                    if (fIcon != null)
                    {
                        string extension = Path.GetExtension(fIcon.FileName);
                        string Newname = "icon_" + Utilities.SEOUrl(category.CatName) + extension;
                        category.Icon = await Utilities.UploadFile(fIcon, @"icons\", Newname.ToLower());
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CatId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CatId == id);
        }
    }
}