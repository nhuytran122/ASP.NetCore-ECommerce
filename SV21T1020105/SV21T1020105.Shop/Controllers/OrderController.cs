using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;
using SV21T1020105.Shop.AppCodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020105.Shop.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PlaceOrder()
        {
            var shoppingCart = ShoppingCartHelper.GetShoppingCart();
            return View(shoppingCart);
        }

        public IActionResult Init(string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = ShoppingCartHelper.GetShoppingCart();

            if (string.IsNullOrWhiteSpace(deliveryProvince))
                ModelState.AddModelError("deliveryProvince", "Vui lòng chọn tỉnh/thành");
            if (string.IsNullOrWhiteSpace(deliveryAddress))
                ModelState.AddModelError("deliveryAddress", "Vui lòng nhập địa chỉ nhận hàng");
            if (!ModelState.IsValid)
            {
                ViewBag.DeliveryProvince = deliveryProvince;
                ViewBag.DeliveryAddress = deliveryAddress;
                return View("PlaceOrder", shoppingCart);
            }
            //TODO:  Thay bởi userID đang login
            //var userData = User.GetUserData();
            //int customerID = int.Parse(userData.UserId.ToString());

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            int orderID = OrderDataService.InitOrder(null, 4194, deliveryProvince, deliveryAddress, orderDetails);
            ShoppingCartHelper.ClearCart();
            // TODO: Thay bởi view Show đơn hàng đang chờ duyệt
            return View("Thanks");
        }

        public IActionResult OrderHistory()
        {
            return View("OrderHistory");
        }

    }
}
