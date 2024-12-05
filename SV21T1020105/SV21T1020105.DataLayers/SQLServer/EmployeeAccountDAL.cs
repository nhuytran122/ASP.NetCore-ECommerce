using Dapper;
using SV21T1020105.DomainModels;
using System.Data;

namespace SV21T1020105.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select	EmployeeID as UserId,
		                            Email as UserName,
		                            FullName as DisplayName,
		                            Photo,
		                            RoleNames
                            from Employees
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
                var sql = @"update Employees
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
    }
}
