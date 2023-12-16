using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Statistical
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult statisticalProduct()
        //{
        //    return View();
        //}


        public ActionResult ThongKeTheoNgay()
        {
            var thongKeData = db.Orders
                .GroupBy(o => DbFunctions.TruncateTime(o.CreatedDate))
                .Select(g => new ThongKeDonHangViewModel { Ngay = g.Key.Value, SoDon = g.Count() })
                .ToList();

            return View(thongKeData);
        }

        public ActionResult ThongKeTheoThang(int selectedMonth)
        {
            var thongKeData = db.Orders
                .Where(o => o.CreatedDate.Month == selectedMonth)
                .GroupBy(o => DbFunctions.TruncateTime(o.CreatedDate))
                .Select(g => new ThongKeDonHangViewModel { Ngay = g.Key.Value, SoDon = g.Count() })
                .ToList();

            return Json(thongKeData, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult GetOrderStatistics()
        {
            var orderStatistics = db.Orders
            .ToList()
            .GroupBy(o => o.CreatedDate.Date)
            .Select(g => new { Date = g.Key, OrderCount = g.Count() })
            .OrderBy(d => d.Date)
            .ToList();


            return Json(orderStatistics, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {

            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.Order.Id
                        join pa in db.ProductAttributes
                        on od.ProductAttributeId equals pa.Id

                        select new
                        {
                            CreateDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = pa.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreateDate >= startDate);

            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreateDate < endDate);

            }
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy,
                TongVon = x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult TopSellingProducts()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetTopSellingProducts()
        {
            var topProducts = db.Products
                .OrderByDescending(p => p.QuantitySold)
                .Take(5) // Lấy 5 sản phẩm bán chạy nhất
                .Select(p => new { p.Title, p.QuantitySold })
                .ToList();

            return Json(topProducts, JsonRequestBehavior.AllowGet);
        }



    }
}