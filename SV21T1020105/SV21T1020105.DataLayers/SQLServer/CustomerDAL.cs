using SV21T1020105.DomainModels;
using Dapper;
using System;
using System.Data;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SV21T1020105.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến Khách hàng (bảng Customers)
    /// </summary>
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1;
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked, Photo)
                                    values (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked, @Photo);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked,
                    Photo = data.Photo ?? ""
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                //Thực thi câu lệnh
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Customers
                            where (CustomerName like @searchValue) or (ContactName like @searchValue)";
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
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Orders where CustomerID = @CustomerID)
	                            select 1
                            else
	                            select 0";
                var parameters = new
                {
                    CustomerID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%"; //"%" + searchValue  + "%"
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
		                            select *,
			                             row_number() over(order by CustomerName) as RowNumber
		                            from Customers
		                            where (CustomerName like @searchValue) or (ContactName like @searchValue)
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
                data = connection.Query<Customer>(sql:sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }

            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerID <> @CustomerID and Email = @Email)
                                begin
                                    update Customers
                                    set CustomerName = @CustomerName,
	                                    ContactName = @ContactName,
	                                    Province = @Province,
	                                    Address = @Address,
	                                    Phone = @Phone,
	                                    Email = @Email,
	                                    IsLocked = @IsLocked,
                                        Photo = @Photo
                                    where CustomerID = @CustomerID
                                end";
                var parameters = new
                {
                   CustomerID = data.CustomerID,
                   CustomerName = data.CustomerName ?? "",
                   ContactName = data.ContactName,
                   Province = data.Province ?? "",
                   Address = data.Address,
                   Phone = data.Phone,
                   Email = data.Email,
                   IsLocked = data.IsLocked,
                   Photo = data.Photo ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Cho phép khách hàng update thông tin cá nhân
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateCustomerProfile(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Customers
                            set CustomerName = @CustomerName,
                                Province = @Province,
                                Address = @Address,
                                Phone = @Phone,
                                Photo = @Photo
                            where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address,
                    Phone = data.Phone,
                    Photo = data.Photo ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

    }
}
