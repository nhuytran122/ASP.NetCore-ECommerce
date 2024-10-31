using SV21T1020105.DomainModels;

namespace SV21T1020105.Web.Models.SearchResults
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}