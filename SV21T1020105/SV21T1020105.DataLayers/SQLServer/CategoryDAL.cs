using Dapper;
using SV21T1020105.DomainModels;
using System.Data;

namespace SV21T1020105.DataLayers.SQLServer
{
    public class CategoryDAL : BaseDAL, ICommonDAL<Category>
    {
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Category data)
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
                            from Categories
                            where (CategoryName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
            }

            return count;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Category? Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool InUsed(int id)
        {
            throw new NotImplementedException();
        }

        public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> data = new List<Category>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from(
                                select *,
                                    row_number() over(order by CategoryName) as RowNumber
                                from Categories
                                where (CategoryName like @searchValue)
                                ) as t
                            where (@pageSize = 0)
                                   or(RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Category>(sql:sql, param:parameters, commandType: CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Category data)
        {
            throw new NotImplementedException();
        }
    }
}
