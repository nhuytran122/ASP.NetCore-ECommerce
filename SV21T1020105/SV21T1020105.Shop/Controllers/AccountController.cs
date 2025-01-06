using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020105.Shop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            ViewBag.UserName = userName;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ tên và mật khẩu");
                return View();
            }

            // Kiểm tra username và password
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, userName, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            // ĐĂNG NHẬP THÀNH CÔNG

            // 1. Tạo ra thông tin "định danh" người dùng

            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            // 2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            // 3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userData = User.GetUserData();

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View();
            }

            bool isChanged = UserAccountService.ChangePassword(UserTypes.Customer, userData.UserName, oldPassword, newPassword);
            if (!isChanged)
            {
                ModelState.AddModelError("Error", "Đổi mật khẩu thất bại. Vui lòng kiểm tra mật khẩu cũ.");
                return View();
            }

            return RedirectToAction("Index", "Profile");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            var data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View(data);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(Customer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Vui lòng nhập tên của bạn");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");
            if (string.IsNullOrWhiteSpace(data.Password))
                ModelState.AddModelError(nameof(data.Password), "Vui lòng nhập mật khẩu");

            if (!ModelState.IsValid)
            {
                return View(data);
            }

            data.ContactName = data.CustomerName;
            int result = CommonDataService.AddCustomer(data);
            if (result == 0)
            {
                ModelState.AddModelError(nameof(data.Email), "Email hiện đã tồn tại. Vui lòng đăng ký với email khác");
                return View(data);
            }
            return RedirectToAction("Login");
        }
    }
}
