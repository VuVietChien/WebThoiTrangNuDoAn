using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class NewsController : Controller
    {
        // GET: Admin/News
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string Searchtext ,int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<News> items = db.News.OrderByDescending(m => m.Id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(m => m.Alias.Contains(Searchtext) || m.Title.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            items = items.ToPagedList(pageIndex,pageSize);
            return View(items);
        }


        public ActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(News model)
        {
            if(ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.CategoryId = 1;
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                db.News.Add(model);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            var item = db.News.Find(Id);
            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(News model)
        {
            if(ModelState.IsValid)
            {
                db.News.Attach(model);
                model.CreatedDate = DateTime.Now;
                model.CategoryId = 1;
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);

                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            var item = db.News.Find(Id);
            if(item!=null)
            {
                db.News.Remove(item);
                db.SaveChanges();

                return Json(new { success = true });

            }
                return Json(new { success = false });

        }


        [HttpPost]
        public ActionResult IsActive(int Id)
        {
            var item = db.News.Find(Id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, isActive = item.IsActive });

            }   
            return Json(new { success = false });

        }



        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {

            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');

                if(items!=null && items.Any())
                {
                    foreach(var item in items)
                    {
                        var obj = db.News.Find(Convert.ToInt32(item));
                        db.News.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });

            }
            return Json(new { success = false });

        }



    }
}