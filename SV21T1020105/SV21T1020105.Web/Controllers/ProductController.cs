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
            if (data.CategoryID == -1)
                ModelState.AddModelError(nameof(data.CategoryID), "Bạn chưa chọn loại hàng của mặt hàng");
            if (data.SupplierID == -1)
                ModelState.AddModelError(nameof(data.SupplierID), "Bạn chưa chọn nhà cung cấp của mặt hàng");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính");
            if (data.Price <= 0)
                ModelState.AddModelError(nameof(data.Price), "Vui lòng nhập giá hàng hợp lệ");

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
                    ModelState.AddModelError(nameof(data.ProductName), "Đã xảy ra lỗi khi thao tác thêm mặt hàng");
                    return View("Edit", data);
                }
                return RedirectToAction("Edit", new {id = id});
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Đã xảy ra lỗi khi thao tác sửa mặt hàng");
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
            ProductPhoto? data;

            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    data = new ProductPhoto(); 
                    data.ProductID = id;
                    return View(data);

                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    data = ProductDataService.GetPhoto(photoId);
                    if (data == null)
                    {
                        return RedirectToAction("Edit", id );
                    }
                    return View(data);

                case "delete":
                    //TODO: Xóa ảnh (xóa trực tiếp, không cần confirm)
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });

                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Thay đổi ảnh của mặt hàng";

            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả hình ảnh không được để trống");

            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không hợp lệ");

            if (ProductDataService.InUsedDisplayOrderOfPhoto(data.ProductID, data.DisplayOrder))
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị hình ảnh đã tồn tại. Vui lòng nhập lại");

            if (uploadPhoto == null)
                ModelState.AddModelError(nameof(data.Photo), "Vui lòng upload ảnh");

            if (!ModelState.IsValid)
            {
                return View("Photo", data);
            }

            string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
            string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                uploadPhoto.CopyTo(stream);
            }
                data.Photo = fileName;
            

            if (data.PhotoID == 0)
            {
                ProductDataService.AddPhoto(data);
            }
            else
            {
                ProductDataService.UpdatePhoto(data);
            }

            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            ProductAttribute? data;

            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    data = new ProductAttribute();
                    data.ProductID = id;
                    return View(data);

                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    data = ProductDataService.GetAttribute(attributeId);
                    if (data == null)
                        return RedirectToAction("Edit", id);
                    return View(data);

                case "delete":
                    //TODO: Xóa thuộc tính (xóa trực tiếp, không cần confirm)
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });

                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");

            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không hợp lệ");
            if (ProductDataService.InUsedDisplayOrderOfAttribute(data.ProductID, data.DisplayOrder) == true)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị thuộc tính đã tồn tại. Vui lòng nhập lại");

            if (!ModelState.IsValid)
            {
                return View("Attribute", data);
            }

            if (data.AttributeID == 0)
            {
                ProductDataService.AddAttribute(data);
            }
            else
            {
                ProductDataService.UpdateAttribute(data);
            }

            return RedirectToAction("Edit", new { id = data.ProductID });
        }

    }
}
