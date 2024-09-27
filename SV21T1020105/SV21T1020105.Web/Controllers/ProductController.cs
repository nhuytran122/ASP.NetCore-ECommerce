using Microsoft.AspNetCore.Mvc;

namespace SV21T1020105.Web.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            return View("Edit");
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
