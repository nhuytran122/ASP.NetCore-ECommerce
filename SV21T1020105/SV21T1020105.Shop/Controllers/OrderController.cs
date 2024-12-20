using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;
using SV21T1020105.Shop.AppCodes;
using SV21T1020105.Shop.Models;
using System.Globalization;

namespace SV21T1020105.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 6;

        private void SetCustomerToViewBag()
        {
            var userData = User.GetUserData(); 
            int customerID = int.Parse(userData.UserId.ToString());

            var data = CommonDataService.GetCustomer(customerID);  
            if (data != null)
            {
                ViewBag.Customer = data; 
            }
        }
        public IActionResult OrderHistory()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddYears(-2).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }

            SetCustomerToViewBag();
            return View(condition);
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

            var userData = User.GetUserData();
            int customerID = int.Parse(userData.UserId.ToString());

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
            int orderID = OrderDataService.InitOrder(null, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ShoppingCartHelper.ClearCart();
            return RedirectToAction("OrderHistory");
        }

        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var userData = User.GetUserData();
            int customerID = int.Parse(userData.UserId.ToString());
            var data = OrderDataService.GetListOrdersByCustomerID(out rowCount, customerID, condition.Page, condition.PageSize,
                                                   condition.Status, condition.FromTime, condition.ToTime);

            // Tạo một danh sách chứa chi tiết các đơn hàng
            var orderDetailModels = new List<OrderDetailModel>();

            // Lặp qua từng đơn hàng và lấy các CTHD
            foreach (var order in data)
            {
                var orderDetails = OrderDataService.ListOrderDetails(order.OrderID); // Lấy CTHD
                var orderDetailModel = new OrderDetailModel
                {
                    Order = order, // gán đơn hàng hiện tại
                    Details = orderDetails.ToList() // gán list CTHD
                };
                orderDetailModels.Add(orderDetailModel); 
            }

            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = orderDetailModels 
            };

            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            SetCustomerToViewBag();

            return View(model);
        }

        public IActionResult Cancel(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("OrderHistory");

            OrderDataService.CancelOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
    }
}
