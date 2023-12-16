using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class ProductAttributeController : Controller
    {
        // GET: Admin/ProductAttribute
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductAttribute()
        {
            var productList = new List<ProductAttributes>
        {
            // Thêm các đối tượng ProductAttributes vào danh sách
            new ProductAttributes {Quantity = 0, Price = 0, PriceSale = 0, OriginalPrice = 0 }, // Thêm thông tin tùy ý
            // Thêm các đối tượng khác tùy ý
        };

            return PartialView("_ProductAttribute", productList);
        }
    }
}