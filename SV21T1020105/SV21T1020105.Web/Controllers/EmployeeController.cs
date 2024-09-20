using Microsoft.AspNetCore.Mvc;

namespace SV21T1020105.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
