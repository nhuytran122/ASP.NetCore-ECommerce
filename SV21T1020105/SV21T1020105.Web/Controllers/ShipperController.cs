using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;

namespace SV21T1020105.Web.Controllers
{
    public class ShipperController : Controller
    {
        public const int PAGE_SIZE = 20;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount;
            var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue ?? "");

            int pageCount = rowCount / PAGE_SIZE;
            if (rowCount % PAGE_SIZE > 0)
                pageCount += 1;

            ViewBag.Page = page;
            ViewBag.RowCount = rowCount;
            ViewBag.PageCount = pageCount;
            ViewBag.SearchValue = searchValue;

            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Người giao hàng";
            var data = new Shipper()
            {
                ShipperID = 0,
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Người giao hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";

            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của người giao hàng");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (data.ShipperID == 0)
            {
                int id = CommonDataService.AddShipper(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateShipper(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }

    }
}
