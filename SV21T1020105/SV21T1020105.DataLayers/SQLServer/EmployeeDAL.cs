using SV21T1020105.DomainModels;
using Dapper;
using System.Data;

namespace SV21T1020105.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu liên quan đến nhân viên (bảng Employees)
    /// </summary>
    public class EmployeeDAL : BaseDAL, ICommonDAL<Employee>
    {
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Employee data)
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
                            from Employees
                            where (FullName like @searchValue)";
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

        public Employee? Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool InUsed(int id)
        {
            throw new NotImplementedException();
        }

        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> data = new List<Employee>();
            searchValue = $"%{searchValue}%"; //"%" + searchValue  + "%"
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
		                            select *,
			                             row_number() over(order by FullName) as RowNumber
		                            from Employees
		                            where (FullName like @searchValue)
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
                data = connection.Query<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }

            return data;
        }

        public bool Update(Employee data)
        {
            throw new NotImplementedException();
        }
    }
}
