using Newtonsoft.Json;
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
    public class ImportOrderController : Controller
    {
        // GET: Admin/ImportOrder
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? page)
        {
            var items = db.ImportOrder.Where(x=>x.Type == 1).OrderByDescending(x => x.CreatedDate).ToList();

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


        public ActionResult Add(ImportOrderDetail Model)
        {
            var productAttributes = db.ProductAttributes.ToList(); // Thay thế 'db' bằng đối tượng DbContext thực tế của bạn

            // Sử dụng Newtonsoft.Json để serialize đối tượng và lưu vào ViewBag
            ViewBag.ProductAttributes = JsonConvert.SerializeObject(productAttributes, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return View();
        }

        [HttpPost]
        public ActionResult AddImportOrderDetails(List<ImportOrderDetail> importOrderDetails, string note, int totalQuantity, float totalPrice)
        {
            var username = HttpContext.User.Identity.Name;
            // Tạo một đối tượng mới để đại diện cho đơn hàng nhập
            var newImportOrder = new ImportOrder
            {
                Note = note,
                TotalProducts = totalQuantity,
                TotalAmount = Convert.ToDecimal(totalPrice),
                Type = 1,
                CreatedBy = username, // Thay bằng người dùng thực tế
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now

            };
            // Thêm đơn hàng nhập mới vào cơ sở dữ liệu

            db.ImportOrder.Add(newImportOrder);
            try
            {
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu để lấy Id của đơn hàng nhập mới
            }
            catch (System.Data.Entity.Core.UpdateException ex)
            {
                var innerException = ex.InnerException;
                if (innerException is System.Data.SqlClient.SqlException sqlException && sqlException.Number == 242)
                {
                    // Chuyển đổi CreatedDate thành kiểu dữ liệu hợp lệ trước khi lưu vào cơ sở dữ liệu
                    newImportOrder.CreatedDate = DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    throw;
                }
            }

            // Lấy Id của đơn hàng nhập mới
            var newImportOrderId = newImportOrder.Id;

            // Cập nhật ImportOrderId cho từng chi tiết đơn hàng nhập
            foreach (var detail in importOrderDetails)
            {
                detail.ImportOrderId = newImportOrderId;
                db.ImportOrderDetail.Add(detail);
                var productAttribute = db.ProductAttributes.Find(detail.ProductAttributeId);
                if (productAttribute != null)
                {
                    productAttribute.Quantity += detail.Quantity; // Cộng thêm số lượng mới
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            return Json(new { success = true, message = "Thêm thành công" });
        }

        public ActionResult SearchProductAttribute(string searchText)
        {
            var productAttributes = db.ProductAttributes
                .Where(pa => pa.Product.Title.Contains(searchText))
                .Select(pa => new { pa.Id, pa.Size, pa.Product.Title, pa.Quantity, pa.OriginalPrice, ProductImage = pa.Product.Image })
                .ToList();

            return Json(productAttributes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View(int id)
        {
            var importOrderDetails = db.ImportOrderDetail.Where(i => i.ImportOrderId == id).ToList();
            var importOrder = db.ImportOrder.Find(id);
            if (importOrder == null)
            {
                // Xử lý khi không tìm thấy đơn hàng nhập
                return RedirectToAction("Index"); // Chuyển hướng đến trang Index nếu không tìm thấy đơn hàng nhập
            }

            foreach (var detail in importOrderDetails)
            {
                var productAttribute = db.ProductAttributes.Find(detail.ProductAttributeId);
                if (productAttribute != null)
                {
                   
                }
            }

            ViewBag.ImportOrderDetails = importOrderDetails;


            return View(importOrder);
        }

        public ActionResult exportProduct(int? page)
        {

            var items = db.ImportOrder.Where(x => x.Type == 0).OrderByDescending(x => x.CreatedDate).ToList();

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


        public ActionResult addExportProduct(ImportOrderDetail Model)
        {
            var productAttributes = db.ProductAttributes.ToList(); // Thay thế 'db' bằng đối tượng DbContext thực tế của bạn

            // Sử dụng Newtonsoft.Json để serialize đối tượng và lưu vào ViewBag
            ViewBag.ProductAttributes = JsonConvert.SerializeObject(productAttributes, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return View();
        }

        [HttpPost]
        public ActionResult addExportProduct(List<ImportOrderDetail> importOrderDetails, string note, int totalQuantity, float totalPrice)
        {
            var username = HttpContext.User.Identity.Name;
            // Tạo một đối tượng mới để đại diện cho đơn hàng nhập
            var newImportOrder = new ImportOrder
            {
                Note = note,
                TotalProducts = totalQuantity,
                TotalAmount = Convert.ToDecimal(totalPrice),
                Type = 0,
                CreatedBy = username, // Thay bằng người dùng thực tế
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now

            };
            // Thêm đơn hàng nhập mới vào cơ sở dữ liệu

            db.ImportOrder.Add(newImportOrder);
            try
            {
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu để lấy Id của đơn hàng nhập mới
            }
            catch (System.Data.Entity.Core.UpdateException ex)
            {
                var innerException = ex.InnerException;
                if (innerException is System.Data.SqlClient.SqlException sqlException && sqlException.Number == 242)
                {
                    // Chuyển đổi CreatedDate thành kiểu dữ liệu hợp lệ trước khi lưu vào cơ sở dữ liệu
                    newImportOrder.CreatedDate = DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    throw;
                }
            }

            // Lấy Id của đơn hàng nhập mới
            var newImportOrderId = newImportOrder.Id;

            // Cập nhật ImportOrderId cho từng chi tiết đơn hàng nhập
            foreach (var detail in importOrderDetails)
            {
                detail.ImportOrderId = newImportOrderId;
                db.ImportOrderDetail.Add(detail);
                var productAttribute = db.ProductAttributes.Find(detail.ProductAttributeId);
                if (productAttribute != null)
                {
                    productAttribute.Quantity -= detail.Quantity; // Cộng thêm số lượng mới
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            return Json(new { success = true, message = "Xuất hàng thành công" });
        }

        public ActionResult viewExport(int id)
        {
            var importOrderDetails = db.ImportOrderDetail.Where(i => i.ImportOrderId == id).ToList();
            var importOrder = db.ImportOrder.Find(id);
            if (importOrder == null)
            {
                // Xử lý khi không tìm thấy đơn hàng nhập
                return RedirectToAction("Index"); // Chuyển hướng đến trang Index nếu không tìm thấy đơn hàng nhập
            }

            foreach (var detail in importOrderDetails)
            {
                var productAttribute = db.ProductAttributes.Find(detail.ProductAttributeId);
                if (productAttribute != null)
                {

                }
            }

            ViewBag.ImportOrderDetails = importOrderDetails;


            return View(importOrder);
        }

    }
}