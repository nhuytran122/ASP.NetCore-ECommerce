using Microsoft.AspNetCore.Mvc;
using SV21T1020105.DomainModels;
using SV21T1020105.Shop.AppCodes;

namespace SV21T1020105.Shop.Controllers
{
    public class CartController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            return View(ShoppingCartHelper.GetShoppingCart());
        }

        public IActionResult ShoppingCart()
        {
            if (ShoppingCartHelper.GetShoppingCart() == null || ShoppingCartHelper.GetShoppingCart().Count() == 0)
            {
                return PartialView("_EmptyCart");
            }
            return View(ShoppingCartHelper.GetShoppingCart());
        }


        public IActionResult AddToCart(CartItem item)
        {
            if (item.Quantity <= 0)
                return Json("Số lượng không hợp lệ");

            var shoppingCart = ShoppingCartHelper.GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = ShoppingCartHelper.GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = ShoppingCartHelper.GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult GetCartCount()
        {
            var shoppingCart = ShoppingCartHelper.GetShoppingCart();
            return Content(shoppingCart.Count.ToString());
        }

        public IActionResult UpdateQuantity(int productID, int quantity)
        {
            if (quantity <= 0)
                return Json("Số lượng không hợp lệ");
            var shoppingCart = ShoppingCartHelper.GetShoppingCart();

            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == productID);
            if (existsProduct != null)
            {
                existsProduct.Quantity = quantity;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
    }
}
