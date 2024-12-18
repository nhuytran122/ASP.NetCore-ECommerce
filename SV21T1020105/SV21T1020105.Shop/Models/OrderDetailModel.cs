using SV21T1020105.DomainModels;

namespace SV21T1020105.Shop.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
