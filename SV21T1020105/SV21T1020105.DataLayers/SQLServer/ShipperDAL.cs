﻿using SV21T1020105.DomainModels;
using Dapper;
using System.Data;

namespace SV21T1020105.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến Nhà cung cấp (bảng Shippers)
    /// </summary>
    public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper data)
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
                            from Shippers
                            where (ShipperName like @searchValue)";
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

        public Shipper? Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool InUsed(int id)
        {
            throw new NotImplementedException();
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> data = new List<Shipper>();
            searchValue = $"%{searchValue}%"; //"%" + searchValue  + "%"
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
		                            select *,
			                             row_number() over(order by ShipperName) as RowNumber
		                            from Shippers
		                            where (ShipperName like @searchValue)
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
                data = connection.Query<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }

            return data;
        }

        public bool Update(Shipper data)
        {
            throw new NotImplementedException();
        }
    }
}