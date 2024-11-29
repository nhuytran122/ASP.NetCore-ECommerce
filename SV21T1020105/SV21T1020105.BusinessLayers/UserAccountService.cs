using SV21T1020105.DataLayers;
using SV21T1020105.DataLayers.SQLServer;
using SV21T1020105.DomainModels;

namespace SV21T1020105.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        static UserAccountService()
        {
            employeeAccountDB = new EmployeeAccountDAL(Configuration.ConnectionString);
            customerAccountDB = new CustomerAccountDAL(Configuration.ConnectionString);
        }

        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }
    }

    public enum UserTypes
    {
        Employee,
        Customer
    }
}
