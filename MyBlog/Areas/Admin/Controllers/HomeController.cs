using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Route("admin.html", Name = "AdminIndex")]
        [Area("Admin")]
        [Authorize()]
        public IActionResult Index()
        {
            return View();
        }
    }
}