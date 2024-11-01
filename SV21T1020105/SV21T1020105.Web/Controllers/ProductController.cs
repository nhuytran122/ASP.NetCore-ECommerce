using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;
using SV21T1020105.Web.Models;
using SV21T1020105.Web.Models.SearchResults;

namespace SV21T1020105.Web.Controllers
{
    public class ProductController : Controller
    {
        public const int PAGE_SIZE = 10;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                };
            return View(condition);
        }

        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var data = new Product()
            {
                ProductID = 0,
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.IsCreateMode = (id == 0);
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính");
            if (data.Price <= 0)
                ModelState.AddModelError(nameof(data.Price), "Giá phải lớn hơn 0");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Đã xảy ra lỗi khi thao tác");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Đã xảy ra lỗi khi thao tác");
                    return View("Edit", data);
                }
            }

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    return View();
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    return View();
                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp, không cần confirm)
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    return View();
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    return View();
                case "delete":
                    //TODO: Xóa thuộc tính (xóa trực tiếp, không cần confirm)
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

    }
}
