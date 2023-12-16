using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_ProductAttributes")]
    public class ProductAttributes
    {
        public ProductAttributes()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.ImportOrderDetail = new HashSet<ImportOrderDetail>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Size không được để trống")]
        public string Size { set; get; }
        public int ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        [Required(ErrorMessage = "Giá sản phẩm không để trống")]
        public decimal Price { get; set; }
        public decimal? PriceSale { get; set; }
        public int? Quantity { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ImportOrderDetail> ImportOrderDetail { get; set; }
    }
}