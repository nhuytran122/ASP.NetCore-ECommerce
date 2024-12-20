using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;

namespace SV21T1020105.Shop.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int customerID = int.Parse(userData.UserId.ToString());
            var data = CommonDataService.GetCustomer(customerID);
            return View(data);
        }

        public IActionResult Edit()
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int customerID = int.Parse(userData.UserId.ToString());
            var data = CommonDataService.GetCustomer(customerID);
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Customer data, IFormFile? uploadPhoto)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Vui lòng nhập họ tên");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            //Xử lý lưu ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(WebConfig.URL_SAVE_IMG, @"images\customers", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            bool result = CommonDataService.UpdateCustomerProfile(data);

            return RedirectToAction("Index");
        }
    }
}
