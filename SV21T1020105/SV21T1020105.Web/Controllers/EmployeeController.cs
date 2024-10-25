using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;
using System.Globalization;

namespace SV21T1020105.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public const int PAGE_SIZE = 16;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue ?? "");

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
            ViewBag.Title = "Bổ sung Nhân viên";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Employee data, string _birthDate, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email của nhân viên");

            if (string.IsNullOrWhiteSpace(_birthDate))
                ModelState.AddModelError(nameof(data.FullName), "Vui lòng nhập ngày sinh của nhân viên");
            
            //Xử lý ngày sinh
            DateTime? d = _birthDate.ToDateTime();
            if (d != null)
            {
                data.BirthDate = d.Value;
            }
            else
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh nhập không hợp lệ");

            if (string.IsNullOrWhiteSpace(data.Phone))
                data.Phone = "";
            if (string.IsNullOrWhiteSpace(data.Address))
                data.Address = "";

            if ((!ModelState.IsValid))
            {
                return View("Edit", data);
            }

            //Xử lý với ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }


            if (data.EmployeeID == 0)
            {
                int id = CommonDataService.AddEmployee(data);
                if(id <= 0){
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateEmployee(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }

}