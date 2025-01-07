using Dapper;
using SV21T1020105.DomainModels;
using System.Data;

namespace SV21T1020105.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select	CustomerID as UserId,
		                            Email as UserName,
		                            CustomerName as DisplayName,
		                            Photo,
		                            N'' as RoleNames,
                                    IsLocked as IsLocked
                            from Customers
                            where	Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string username, string password, string newPassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Customers
                            set Password = @NewPassword
                            where Email = @Email and Password = @OldPassword";

                var parameters = new
                {
                    Email = username,
                    OldPassword = password,
                    NewPassword = newPassword
                };

                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public int Register(Customer data, string password)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1;
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked, Photo, Password)
                                    values (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked, @Photo, @Password);
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
                    Photo = data.Photo ?? "",
                    Password = password ?? ""
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
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
