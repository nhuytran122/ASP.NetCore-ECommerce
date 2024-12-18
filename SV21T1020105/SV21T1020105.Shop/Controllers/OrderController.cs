using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;
using SV21T1020105.Shop.Models;
using SV21T1020105.Shop.AppCodes;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020105.Shop.Controllers
{
    public class OrderController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 20;
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

        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.GetListOrdersByCustomerID(out rowCount, 4781, condition.Page, condition.PageSize,
                                                   condition.Status, condition.FromTime, condition.ToTime);

            // Tạo một danh sách chứa chi tiết các đơn hàng
            var orderDetailModels = new List<OrderDetailModel>();

            // Lặp qua từng đơn hàng và lấy các chi tiết của từng đơn hàng
            foreach (var order in data)
            {
                var orderDetails = OrderDataService.ListOrderDetails(order.OrderID); // Lấy chi tiết đơn hàng
                var orderDetailModel = new OrderDetailModel
                {
                    Order = order, // gán đơn hàng hiện tại
                    Details = orderDetails.ToList() // gán list CTHD
                };
                orderDetailModels.Add(orderDetailModel); 
            }

            // Tạo model cho kết quả tìm kiếm
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
            return View(model); // Trả về view với model chứa thông tin chi tiết
        }

        //public IActionResult OrderHistory()
        //{
        //    //var order = OrderDataService.GetOrder(1477);
        //    //if (order == null)
        //    //    return RedirectToAction("Index");

        //    //var details = OrderDataService.GetListOrdersByCustomerID(1477);
        //    //var model = new OrderDetailModel()
        //    //{
        //    //    Order = order,
        //    //    Details = details
        //    //};

        //    return View();
        //}

    }
}
