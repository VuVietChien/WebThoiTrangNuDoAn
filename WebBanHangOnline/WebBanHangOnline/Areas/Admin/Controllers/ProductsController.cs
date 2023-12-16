using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Products
        public ActionResult Index(string Searchtext,int? page)
        {
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(m => m.Alias.Contains(Searchtext) || m.Title.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;

            foreach(var item in items)
            {
                UpdateTotalProduct(item.Id);
            }    
            return View(items);
        }

        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model,List<ProductAttributes> productAttributes, List<string> Images, List<int> rDefault)
        {
            if (ModelState.IsValid)
            {
                if (Images != null && Images.Count > 0)

                {
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (i + 1 == rDefault[0])
                        {
                            model.Image = Images[i];
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductId = model.Id,
                                Image = Images[i],  
                                IsDefault = true
                            });
                        }
                        else
                        {
                            model.ProductImage.Add(new ProductImage
                            {
                                ProductId = model.Id,
                                Image = Images[i],
                                IsDefault = false
                            });
                        }
                    }
                }
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.SeoTitle))
                {
                    model.SeoTitle = model.Title;
                }
                if (string.IsNullOrEmpty(model.Alias))
                    model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                db.Products.Add(model);
                //UpdateTotalProduct(model.Id);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            return View(model);
        }

        public ActionResult ExportToExcel(string searchText)
        {
            // Lấy dữ liệu từ cơ sở dữ liệu hoặc một nguồn dữ liệu khác
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id).ToList();

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchText))
            {
                items = items.Where(m => m.Alias.Contains(searchText) || m.Title.Contains(searchText));
            }

            // Tạo một package Excel sử dụng thư viện EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachDonHang");

                // Thêm các tiêu đề cột
                worksheet.Cells[1, 1].Value = "Tên sản phẩm";
                worksheet.Cells[1, 2].Value = "Danh mục sản phẩm";
                worksheet.Cells[1, 3].Value = "Ngày tạo";
                worksheet.Cells[1, 4].Value = "Đường dẫn hình ảnh";
                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 60;
                // Thêm các cột khác tùy thuộc vào yêu cầu của bạn

                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.GreenYellow);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    range.Style.Font.Bold = true;

                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                // Đưa dữ liệu vào từng ô trong bảng
                int row = 2;

                foreach (var product in items)
                {
                    //var productCategoryId = product.ProductCategoryId;
                    //var item = db.ProductCategories.Find(productCategoryId);
                    //var productCategory = item.Title;
                    worksheet.Cells[row, 1].Value = product.Title;
                    //worksheet.Cells[row, 2].Value = product.Title;
                    worksheet.Cells[row, 2].Value = product.ProductCategory.Title;
                    //worksheet.Cells[row, 2].Value = productCategory;
                    worksheet.Cells[row, 3].Value = product.CreatedDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = product.Image;
                   

                    // Thêm đường viền cho từng ô
                    using (var cellRange = worksheet.Cells[row, 1, row, 4])
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
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachSanPham.xlsx");
            }
        }
        public ActionResult Edit(int id)
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            var item = db.Products.Find(id);
            var productAttributes = db.ProductAttributes.Where(pa => pa.ProductId == id).ToList();
            item.ProductAttributes = productAttributes;
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, List<ProductAttributes> productAttributes)
        {

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            product.ModifiedDate = DateTime.Now;
            // Cập nhật thông tin sản phẩm trong cơ sở dữ liệu
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            // Cập nhật thông tin ProductAttribute trong cơ sở dữ liệu
            foreach (var attribute in productAttributes)
            {
                db.Entry(attribute).State = EntityState.Modified;
            }
            db.SaveChanges();
            UpdateTotalProduct(product.Id);

            return RedirectToAction("Index");
            //if (ModelState.IsValid)
            //{
            //    model.ModifiedDate = DateTime.Now;
            //    model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
            //    db.Products.Attach(model);
            //    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                var checkImg = item.ProductImage.Where(x => x.ProductId == item.Id);
                if (checkImg != null)
                {
                    foreach(var img in checkImg)
                    {
                        db.ProductImages.Remove(img);
                        db.SaveChanges();
                    }
                }
                db.Products.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int Id)
        {
            var item = db.Products.Find(Id);
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
        public ActionResult IsHome(int Id)
        {
            var item = db.Products.Find(Id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, isHome = item.IsHome });

            }
            return Json(new { success = false });

        }

        [HttpPost]
        public ActionResult IsSale(int Id)
        {
            var item = db.Products.Find(Id);
            if (item != null)
            {
                item.IsSale = !item.IsSale;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, isSale = item.IsSale });

            }
            return Json(new { success = false });

        }

        public void UpdateTotalProduct(int productId)
        {
            var product = db.Products.Find(productId);
            if (product != null)
            {
                // Tính tổng số lượng từ ProductAttributes
                int totalProduct = (int)db.ProductAttributes
                    .Where(pa => pa.ProductId == productId)
                    .Sum(pa => pa.Quantity);

                // Cập nhật trường totalProduct trong bảng Product
                product.TotalProduct = totalProduct;

                // Lưu các thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
            }
        }
    }
}