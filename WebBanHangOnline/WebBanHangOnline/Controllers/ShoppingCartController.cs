using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Hubs;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        public ActionResult CheckOutSuccess()
        {
            return View();
        }
        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }


        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Partial_CheckOut()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel req)
        {
            var code = new { Success = false, Code = -1 };
            if (ModelState.IsValid)
            {

                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    Order order = new Order();
                    order.CustomerName = req.CustomerName;
                    order.Phone = req.Phone;
                    order.Address = req.Address;
                    cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    {
                        ProductAttributeId = x.ProductAttributeId,
                        Quantity = x.Quantity,
                        Price = x.Price

                        // update số lượng hàng đã bán

                    }));

                    foreach(var item in cart.Items)
                    {
                        var username = HttpContext.User.Identity.Name;
                        // Tạo một đối tượng mới để đại diện cho đơn hàng nhập
                        var newImportOrder = new ImportOrder
                        {
                            Note = "Khách mua online",
                            TotalProducts = item.Quantity,
                            TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity)),
                            Type = 0,
                            CreatedBy = username, // Thay bằng người dùng thực tế
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now

                        };
                        // Thêm đơn hàng nhập mới vào cơ sở dữ liệu

                        db.ImportOrder.Add(newImportOrder);
                        db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu để lấy Id của đơn hàng nhập mới

                        var productAttribute = db.ProductAttributes.FirstOrDefault(p => p.Id == item.ProductAttributeId);
                        if (productAttribute != null)
                        {
                            productAttribute.Quantity -= item.Quantity;
                            db.SaveChanges();
                        }
                        var product = db.Products.FirstOrDefault(p => p.Id == productAttribute.ProductId);
                        if (product != null)
                        {
                            product.QuantitySold += item.Quantity;
                            db.SaveChanges();
                        }
                    }
                    order.StatusOrder = 1;
                    order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                    order.TypePayment = req.TypePayment;
                    order.Email = req.Email;
                    order.CreatedDate = DateTime.Now;
                    order.ModifiedDate = DateTime.Now;
                    order.CreatedBy = req.Phone;
                    Random rd = new Random();
                    order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                    db.Orders.Add(order);
                    db.SaveChanges();
                    NotificationHub.NotifyAdmin("Chúc mừng bạn, có đơn hàng mới!");



                    // send mail cho khach hang
                    var strSanPham = "";
                    var thanhTien = decimal.Zero;
                    var tongTien = decimal.Zero;
                    foreach (var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        strSanPham += "<td>" + sp.ProductName + "</td>";
                        strSanPham += "<td>" + sp.Size + "</td>";
                        strSanPham += "<td>" + sp.Quantity + "</td>";
                        strSanPham += "<td>" + sp.Price.ToString() + "</td>";
                        strSanPham += "</tr>";
                        thanhTien += sp.Price * sp.Quantity;
                    }
                    tongTien = thanhTien;
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{Email}}", req.Email);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", thanhTien.ToString());
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", tongTien.ToString());
                    WebBanHangOnline.Common.Common.SendMail("ShineFashion", "Đơn Hàng #" + order.Code, contentCustomer.ToString(), req.Email);

                    // send mail cho Admin

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                    contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
                    contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", thanhTien.ToString());
                    contentAdmin = contentAdmin.Replace("{{TongTien}}", tongTien.ToString());
                    WebBanHangOnline.Common.Common.SendMail("ShineFashion", "Đơn Hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);

                    cart.ClearCart();
                    return RedirectToAction("CheckOutSuccess");

                }
            }
            return Json(code);
        }
        //public ActionResult CheckOut(OrderViewModel req)
        //{
        //    var code = new { Success = false, Code = -1 };
        //    if (ModelState.IsValid)
        //    {

        //        ShoppingCart cart = (ShoppingCart)Session["Cart"];
        //        if (cart != null)
        //        {
        //            Order order = new Order();
        //            order.CustomerName = req.CustomerName;
        //            order.Phone = req.Phone;
        //            order.Address = req.Address;
        //            cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
        //            {
        //                ProductId = x.ProductId,
        //                Quantity = x.Quantity,
        //                Price = x.Price

        //            }));

        //            order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
        //            order.TypePayment = req.TypePayment;
        //            order.Email = req.Email;
        //            order.CreatedDate = DateTime.Now;
        //            order.ModifiedDate = DateTime.Now;
        //            order.CreatedBy = req.Phone;
        //            Random rd = new Random();
        //            order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
        //            db.Orders.Add(order);
        //            db.SaveChanges();

        //            // send mail cho khach hang
        //            var strSanPham = "";
        //            var thanhTien = decimal.Zero;
        //            var tongTien = decimal.Zero;
        //            foreach (var sp in cart.Items)
        //            {
        //                strSanPham += "<tr>";
        //                strSanPham += "<td>" + sp.ProductName + "</td>";
        //                strSanPham += "<td>" + sp.Quantity + "</td>";
        //                strSanPham += "<td>" + sp.Price.ToString() + "</td>";
        //                strSanPham += "</tr>";
        //                thanhTien += sp.Price * sp.Quantity;
        //            }
        //            tongTien = thanhTien;
        //            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send2.html"));
        //            contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
        //            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
        //            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
        //            contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
        //            contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
        //            contentCustomer = contentCustomer.Replace("{{Email}}", req.Email);
        //            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
        //            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", thanhTien.ToString());
        //            contentCustomer = contentCustomer.Replace("{{TongTien}}", tongTien.ToString());
        //            WebBanHangOnline.Common.Common.SendMail("ShineFashion", "Đơn Hàng #" + order.Code, contentCustomer.ToString(), req.Email);

        //            // send mail cho Admin

        //            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send1.html"));
        //            contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
        //            contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
        //            contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
        //            contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
        //            contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
        //            contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
        //            contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
        //            contentAdmin = contentAdmin.Replace("{{ThanhTien}}", thanhTien.ToString());
        //            contentAdmin = contentAdmin.Replace("{{TongTien}}", tongTien.ToString());
        //            WebBanHangOnline.Common.Common.SendMail("ShineFashion", "Đơn Hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);

        //            cart.ClearCart();
        //            return RedirectToAction("CheckOutSuccess");

        //        }
        //    }
        //    return Json(code);
        //}

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity, int attributeId)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
            var checkProductAttribute = db.ProductAttributes.FirstOrDefault(x => x.Id == attributeId);
            if(checkProductAttribute == null)
            {
                checkProductAttribute = db.ProductAttributes.FirstOrDefault(x => x.ProductId == id);
            }
            if (checkProductAttribute != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.Id,
                    ProductAttributeId = checkProductAttribute.Id,
                    ProductName = checkProduct.Title,
                    CategoryName = checkProduct.ProductCategory.Title,
                    Alias = checkProduct.Alias,
                    Size = checkProductAttribute.Size,
                    Quantity = quantity,
                    //Price = checkProductAttribute.Price
                };
                if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                {
                    item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                }
                item.Price = checkProductAttribute.Price;
                if (checkProductAttribute.PriceSale > 0)
                {
                    item.Price = (decimal)checkProductAttribute.PriceSale;
                }
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giở hàng thành công!", code = 1, Count = cart.Items.Count };
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProductAttribute = cart.Items.FirstOrDefault(x => x.ProductAttributeId == id);
                if (checkProductAttribute != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }



        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
    }
}