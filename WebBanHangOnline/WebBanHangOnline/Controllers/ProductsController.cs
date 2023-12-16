using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(int? id)
        {
            var items = db.Products.ToList();
            if (id != null)
            {
                items = db.Products.Where(x => x.ProductCategoryId == id).ToList();
            }
           
            return View(items);
        }

        public ActionResult Detail(string alias,int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            if (item.ProductAttributes != null && item.ProductAttributes.Any())
            {
                ViewBag.Price = item.ProductAttributes.First().Price;
                ViewBag.PriceSale = item.ProductAttributes.First().PriceSale;
                ViewBag.OriginalPrice = item.ProductAttributes.First().OriginalPrice;
            }

            return View(item);
            
        }
        public ActionResult ProductCategory(string alias,int id)
        {
            var items = db.Products.ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }

            ViewBag.CateId = id;
            return View(items);
        }

        public ActionResult Partial_ItemsByCateId()
        {
            var items = db.Products.Where(x => x.IsHome && x.IsActive).Take(12).ToList();
            //var items = db.Products.Where(x => x.IsHome && x.IsActive == true).Take(12).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ProductSales()
        {
            var items = db.Products.Where(x=>x.IsActive == true).OrderByDescending(x => x.QuantitySold).Take(12).ToList();
            return PartialView(items);
        }
    }
}