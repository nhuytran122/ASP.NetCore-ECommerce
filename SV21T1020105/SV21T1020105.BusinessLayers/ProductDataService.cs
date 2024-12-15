using SV21T1020105.DataLayers.SQLServer;
using SV21T1020105.DataLayers;
using SV21T1020105.DomainModels;

namespace SV21T1020105.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng (không phân trang)
        /// </summary>
        public static List<Product> ListProducts(string searchValue = "") 
        {  
            return productDB.List(searchValue: searchValue).ToList();
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0,
                                                 string searchValue = "", int categoryId = 0, int supplierId = 0,
                                                 decimal minPrice = 0, decimal maxPrice = 0, string sortByPrice = "")
        { 
            rowCount = productDB.Count(searchValue, categoryId, supplierId, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice, sortByPrice).ToList();
        }

        /// <summary>
        /// Lấy thông tin 1 mặt hàng theo mã mặt hàng
        /// </summary>
        public static Product? GetProduct(int productID) 
        { 
            return productDB.Get(productID);
        }

        /// <summary>
        /// Thêm một mặt hàng mới
        /// </summary>
        public static int AddProduct(Product data) 
        {  
            return productDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin một mặt hàng
        /// </summary>
        public static bool UpdateProduct(Product data) 
        { 
            return productDB.Update(data);
        }

        /// <summary>
        /// Xóa một mặt hàng
        /// </summary>
        public static bool DeleteProduct(int productID) 
        {
            return productDB.Delete(productID); 
        }

        /// <summary>
        /// Kiểm tra xem mặt hàng có được sử dụng không
        /// </summary>
        public static bool InUsedProduct(int productID) 
        {
            return productDB.InUsed(productID);
        }

        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng
        /// </summary>
        public static List<ProductPhoto> ListPhotos(int productID) 
        {
            return productDB.ListPhotos(productID).ToList();
        }

        /// <summary>
        /// Lấy thông tin ảnh của mặt hàng
        /// </summary>
        public static ProductPhoto? GetPhoto(long photoID) 
        {
            return productDB.GetPhoto(photoID);
        }

        /// <summary>
        /// Thêm ảnh cho mặt hàng
        /// </summary>
        public static long AddPhoto(ProductPhoto data) 
        {
            return productDB.AddPhoto(data);
        }

        /// <summary>
        /// Cập nhật ảnh cho mặt hàng
        /// </summary>
        public static bool UpdatePhoto(ProductPhoto data) 
        {
            return productDB.UpdatePhoto(data);
        }

        /// <summary>
        /// Xóa ảnh của mặt hàng
        /// </summary>
        public static bool DeletePhoto(long photoID)
        {  
            return productDB.DeletePhoto(photoID);
        }

        /// <summary>
        /// Lấy danh sách thuộc tính của mặt hàng
        /// </summary>
        public static List<ProductAttribute> ListAttributes(int productID) 
        {  
            return productDB.ListAttributes(productID).ToList();
        }

        /// <summary>
        /// Lấy thông tin thuộc tính của mặt hàng
        /// </summary>
        public static ProductAttribute? GetAttribute(int attributeID) 
        {  
            return productDB.GetAttribute(attributeID);
        }

        /// <summary>
        /// Thêm thuộc tính cho mặt hàng
        /// </summary>
        public static long AddAttribute(ProductAttribute data)
        { 
            return productDB.AddAttribute(data);
        }

        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }

        /// <summary>
        /// Xóa thuộc tính của mặt hàng
        /// </summary>
        public static bool DeleteAttribute(long attributeID) 
        {  
            return productDB.DeleteAttribute(attributeID);
        }

        public static bool InUsedDisplayOrderOfAttribute(int productID, int displayOrder)
        {
            return productDB.DisplayOrderOfAttributeInUsed(productID, displayOrder);
        }

        public static bool InUsedDisplayOrderOfPhoto(int productID, int displayOrder)
        {
            return productDB.DisplayOrderOfPhotoInUsed(productID, displayOrder);
        }
    }

}
