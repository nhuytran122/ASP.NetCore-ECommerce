using SV21T1020105.DomainModels;

namespace SV21T1020105.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}