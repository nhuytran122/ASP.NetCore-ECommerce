using SV21T1020105.DomainModels;

namespace SV21T1020105.DataLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;

        static CommonDataService()
        {
            string connectionString = @"server=DESKTOP-6HUBDCV;user id=sa;password=nhuytran;database=LiteCommerceDB;TrustServerCertificate=true";
            customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name=""></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }
    }    
}

/*
 Lớp static là gì? Khác lớp không phải là static chỗ nào?
Constructor trong lớp static sử dụng thế nào? Khác constructor của lớp thông thường chỗ nào?
 */