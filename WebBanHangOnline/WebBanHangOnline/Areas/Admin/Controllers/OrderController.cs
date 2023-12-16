using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using PagedList;
using System.Globalization;
using System.Data.Entity;
using WebBanHangOnline.Models.ViewModels;
using WebBanHangOnline.Models.EF;
using OfficeOpenXml;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class OrderController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index(string Searchtext, int? page)
        {
            IEnumerable<Order> items = db.Orders.OrderByDescending(x => x.CreatedDate).ToList();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(m => m.Code.Contains(Searchtext) || m.CustomerName.Contains(Searchtext));
            }
            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult ExportToExcel(string searchText)
        {
            // Lấy dữ liệu từ cơ sở dữ liệu hoặc một nguồn dữ liệu khác
            IEnumerable<Order> orders = db.Orders.OrderByDescending(x => x.CreatedDate);

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchText))
            {
                orders = orders.Where(m => m.Code.Contains(searchText) || m.CustomerName.Contains(searchText));
            }

            // Tạo một package Excel sử dụng thư viện EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachDonHang");

                // Thêm các tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã đơn hàng";
                worksheet.Cells[1, 2].Value = "Tên khách hàng";
                worksheet.Cells[1, 3].Value = "Số điện thoại";
                worksheet.Cells[1, 4].Value = "Tổng tiền";
                worksheet.Cells[1, 5].Value = "Trạng thái thanh toán";
                worksheet.Cells[1, 6].Value = "Ngày tạo";
                worksheet.Cells[1, 7].Value = "Trạng thái đơn hàng";
                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 30;
                // Thêm các cột khác tùy thuộc vào yêu cầu của bạn

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    range.Style.Font.Bold = true;

                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                // Đưa dữ liệu vào từng ô trong bảng
                int row = 2;

                foreach (var order in orders)
                {
                    worksheet.Cells[row, 1].Value = order.Code;
                    worksheet.Cells[row, 2].Value = order.CustomerName;
                    worksheet.Cells[row, 3].Value = order.Phone;
                    worksheet.Cells[row, 4].Value = order.TotalAmount.ToString();
                    worksheet.Cells[row, 5].Value = order.TypePayment == 1 ? "Chờ thành toán" : "Đã thanh toán";
                    worksheet.Cells[row, 6].Value = order.CreatedDate.ToString("dd/MM/yyyy");
                    var orderStatusStr = "";
                    switch (order.StatusOrder)
                    {
                        case 1:
                            orderStatusStr = "Chờ xác nhận";
                            break;
                        case 2:
                            orderStatusStr = "Đã xác nhận";
                            break;
                        case 3:
                            orderStatusStr = "Đã gửi hàng";
                            break;
                        case 4:
                            orderStatusStr = "Đã nhận hàng";
                            break;
                        case 5:
                            orderStatusStr = "Đang hoàn";
                            break;
                        case 6:
                            orderStatusStr = "Đã hoàn thành";
                            break;
                    }
                    worksheet.Cells[row, 7].Value = orderStatusStr;

                    // Thêm đường viền cho từng ô
                    using (var cellRange = worksheet.Cells[row, 1, row, 7])
                    {
                        cellRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        cellRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        cellRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        cellRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    // Thêm các ô dữ liệu khác tùy thuộc vào yêu cầu của bạn
                    row++;
                }

                byte[] excelData = package.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachDonHang.xlsx");
            }
        }


        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            foreach (var orderDetail in items)
            {
                var productAttribute = db.ProductAttributes.Find(orderDetail.ProductAttributeId);
                if (productAttribute != null)
                {
                    var product = db.Products.Find(productAttribute.ProductId);
                    if (product != null)
                    {
                        if (orderDetail.ProductAttributes == null)
                        {
                            orderDetail.ProductAttributes = new ProductAttributes();
                        }
                        orderDetail.ProductAttributes.Product = product;
                        orderDetail.ProductAttributes.Size = productAttribute.Size;
                    }
                }
            }
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai, int statusOrder)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                item.StatusOrder = statusOrder;
                item.TypePayment = trangthai;
                db.Entry(item).Property(x => x.TypePayment).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }

        public void ThongKe(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join ap in db.ProductAttributes
                        on od.ProductAttributeId equals ap.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = ap.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= start);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate < endDate);
            }
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(r => new
            {
                Date = r.Key.Value,
                TotalBuy = r.Sum(x => x.OriginalPrice * x.Quantity), // tổng giá bán
                TotalSell = r.Sum(x => x.Price * x.Quantity) // tổng giá mua
            }).Select(x => new RevenueStatisticViewModel
            {
                Date = x.Date,
                Benefit = x.TotalSell - x.TotalBuy,
                Revenues = x.TotalSell
            });
        }
    }
}