using System.ComponentModel.DataAnnotations;

namespace SV21T1020105.DomainModels
{
    public class ProductPhoto
    {
        public long PhotoID { get; set; }
        public int ProductID { get; set; }
        public string Photo { get; set; } = "";
        public string Description { get; set; } = "";
        [Required(ErrorMessage = "Vui lòng nhập thứ tự hiển thị của hình ảnh")]
        public int? DisplayOrder { get; set; }
        public bool IsHidden { get; set; }
    }
}
