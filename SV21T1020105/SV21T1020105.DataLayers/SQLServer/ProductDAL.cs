using Dapper;
using SV21T1020105.DomainModels;
using System.Data;

namespace SV21T1020105.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                            values (@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                            select SCOPE_IDENTITY();";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                //Thực thi câu lệnh
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                            values (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                            select SCOPE_IDENTITY();";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue,
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                //Thực thi câu lệnh
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                            values (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                            select SCOPE_IDENTITY();";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                //Thực thi câu lệnh
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*)
                    FROM Products
                    WHERE (@CategoryID = 0 OR CategoryID = @CategoryID)
                      AND (@SupplierID = 0 OR SupplierID = @SupplierID)
                      AND (Price >= @MinPrice)
                      AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                      AND (@SearchValue = N'' OR ProductName LIKE @SearchValue)";
                var parameters = new
                {
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SearchValue = searchValue
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where ProductID = @ProductID
                            delete from ProductPhotos where ProductID = @ProductID
                            delete from Products where ProductID = @ProductID";
                            
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DisplayOrderOfAttributeInUsed(int productID, int displayOrder)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from ProductAttributes where ProductID = @ProductID AND DisplayOrder = @DisplayOrder)
                                select 1
                            else
                                select 0";
                var parameters = new
                {
                    ProductID = productID,
                    DisplayOrder = displayOrder
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public bool DisplayOrderOfPhotoInUsed(int productID, int displayOrder)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from ProductPhotos where ProductID = @ProductID AND DisplayOrder = @DisplayOrder)
                                select 1
                            else
                                select 0";
                var parameters = new
                {
                    ProductID = productID,
                    DisplayOrder = displayOrder
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from OrderDetails where ProductID = @ProductID)
                                select 1
                            else
                                select 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM (
                            SELECT *,
                            ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
                            FROM Products
                            WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                            AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                            AND (@SupplierID = 0 OR SupplierID = @SupplierID)
                            AND (Price >= @MinPrice)
                            AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                            ) AS t
                            WHERE (@PageSize = 0)
                            OR (RowNumber BETWEEN (@Page - 1)*@PageSize + 1 AND @Page * @PageSize)";
                var parameters = new
                {
                    Page = page,
                    PageSize = pageSize,
                    SearchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MaxPrice = maxPrice,
                    MinPrice = minPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
            }
            return data;
        }

        public List<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM
                            ProductAttributes
                            WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
            }
            return data;
        }

        public List<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM
                            ProductPhotos
                            WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Products
                            set ProductName = @ProductName,
	                        ProductDescription = @ProductDescription,
	                        SupplierID = @SupplierID,
	                        CategoryID = @CategoryID,
	                        Unit = @Unit,
                            Price = @Price,
	                        Photo = @Photo,
                            IsSelling = @IsSelling
                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                    ProductID = data.ProductID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductAttributes
                            set AttributeName = @AttributeName,
	                        AttributeValue = @AttributeValue,
	                        DisplayOrder = @DisplayOrder
                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductPhotos
                            set Photo = @Photo,
	                        Description = @Description,
	                        DisplayOrder = @DisplayOrder,
                            IsHidden = @IsHidden
                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}

