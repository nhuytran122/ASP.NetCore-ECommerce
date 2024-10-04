using Microsoft.Data.SqlClient;

namespace SV21T1020105.DataLayers.SQLServer
{
    public abstract class BaseDAL
    {
        protected string _connectionString = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Chuỗi lưu trữ các tham số kết nối đến CSDL</param>
        public BaseDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Tạo và mở 1 kết nối đến CSDL (SQL Server)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

    }
}
