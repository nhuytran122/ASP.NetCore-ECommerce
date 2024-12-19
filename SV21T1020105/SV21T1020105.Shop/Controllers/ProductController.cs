using Microsoft.AspNetCore.Mvc;
using SV21T1020105.BusinessLayers;
using SV21T1020105.DomainModels;
using SV21T1020105.Shop.Models;
using System.Buffers;

namespace SV21T1020105.Shop.Controllers
{
    public class ProductController : Controller
    {
        public const int PAGE_SIZE = 9;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                    SortByPrice = ""
                };
            return View(condition);
        }

        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", 
                condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice, condition.SortByPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                SortByPrice = condition.SortByPrice,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

            return View(model);
        }
    
        public IActionResult Details(int id = 0)
        {
            var product = ProductDataService.GetProduct(id);
            if (product == null)
                return RedirectToAction("Index");

            var attributes = ProductDataService.ListAttributes(id);
            var photos = ProductDataService.ListPhotos(id);
            var model = new ProductDetailModel()
            {
                Product = product,
                Attributes = attributes,
                Photos = photos,
                CategoryName = CommonDataService.GetCategory(product.CategoryID).CategoryName
            };

            return View(model);
        }

        public IActionResult Filter(int categoryID = 0)
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION)
                            ?? new ProductSearchInput();

            condition.Page = 1;
            condition.PageSize = PAGE_SIZE;
            condition.SearchValue = "";
            condition.CategoryID = categoryID;
            condition.SupplierID = 0;
            condition.MinPrice = 0;
            condition.MaxPrice = 0;
            condition.SortByPrice = "";

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

            return RedirectToAction("Index");
        }
    }
}
