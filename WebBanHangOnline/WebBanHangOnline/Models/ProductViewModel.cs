using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<ProductAttributes> ProductAttributes { get; set; }
        public string Name { set; get; }
        public int ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceSale { get; set; }
        public int Quantity { get; set; }
    }
}