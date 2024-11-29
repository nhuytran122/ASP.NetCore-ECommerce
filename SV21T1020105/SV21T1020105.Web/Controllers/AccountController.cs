using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;

namespace SV21T1020105.Web.Controllers
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

            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ tên và mật khẩu");
                return View();
            }

            // TODO: Kiểm tra xem username và password (của Employee) có đúng hay không?
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, userName, password);
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
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
