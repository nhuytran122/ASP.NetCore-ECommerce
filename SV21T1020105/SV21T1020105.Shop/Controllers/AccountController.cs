using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;

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
            int customerID = int.Parse(userData.UserId.ToString());

            var data = CommonDataService.GetCustomer(customerID);
            if (data != null)
            {
                ViewBag.Customer = data;
            }

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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
