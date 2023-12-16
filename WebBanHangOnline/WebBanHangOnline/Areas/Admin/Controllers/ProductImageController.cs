using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class ProductImageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ProductImage
        public ActionResult Index(int id)
        {
            ViewBag.ProductId = id;
            var items = db.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(items);
        }

        [HttpPost]
        public ActionResult AddImage(int productId, string url)
        {
            var items = db.ProductImages.Where(x => x.ProductId == productId).ToList();
            var product = db.Products.Find(productId);
            if (items.Count == 0)
            {
                db.ProductImages.Add(new ProductImage
                {
                    ProductId = productId,
                    Image = url,
                    IsDefault = true,

                });
                product.Image = items[0].Image;
            }
            else
            {
                db.ProductImages.Add(new ProductImage
                {
                    ProductId = productId,
                    Image = url,
                    IsDefault = false,

                });
                product.Image = items[0].Image;
            }
            db.SaveChanges();
            return Json(new { Success = true });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.ProductImages.Find(id);
            db.ProductImages.Remove(item);
            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult IsDefault(int ProductId, int id)
        {
            var item1 = db.ProductImages.Where(x => x.ProductId == ProductId).ToList();
            foreach (var i in item1)
            {
                i.IsDefault = false;
            }

            var item = db.ProductImages.Find(id);
            item.IsDefault = true;
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}