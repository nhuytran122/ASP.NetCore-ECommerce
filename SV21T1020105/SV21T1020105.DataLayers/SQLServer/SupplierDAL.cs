using SV21T1020105.DomainModels;
using Dapper;
using System;
using System.Data;

namespace SV21T1020105.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến Nhà cung cấp (bảng Suppliers)
    /// </summary>
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            throw new NotImplementedException();
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Suppliers
                            where (SupplierName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            }
            return count;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Supplier? Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool InUsed(int id)
        {
            throw new NotImplementedException();
        }

        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            searchValue = $"%{searchValue}%"; //"%" + searchValue  + "%"
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
		                            select *,
			                             row_number() over(order by SupplierName) as RowNumber
		                            from Suppliers
		                            where (SupplierName like @searchValue) or (ContactName like @searchValue)
		                            )	as t
                            where (@pageSize = 0)
	                            or(RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue // bên trái: tên cảu tham số trong câu lệnh SQL, bên phải: giá trị truyền cho tham số
                };
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }

            return data;
        }

        public bool Update(Supplier data)
        {
            throw new NotImplementedException();
        }
    }
}
