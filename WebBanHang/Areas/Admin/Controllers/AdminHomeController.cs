using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.EF;
using System.Configuration;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class AdminHomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var today = DateTime.Today;
                        var tomorrow = today.AddDays(1);
ViewBag.TotalOrdersToday = db.Orders.Count(o => o.CreatedDate.HasValue && o.CreatedDate.Value >= today && o.CreatedDate.Value < tomorrow);
            ViewBag.TotalInventory = db.Products.Sum(p => p.Quantity);
            ViewBag.TotalInventoryValue = db.Products.Sum(p => p.Quantity * p.Price);
            
            // Get pending orders (Status = 0)
            var pendingOrders = db.Orders
                .Where(o => o.Status == 0)
                .OrderByDescending(o => o.CreatedDate)
                .ToList();

            return View(pendingOrders);
        }

        [HttpPost]
        public ActionResult ConfirmOrder(int id)
        {
            var order = db.Orders.Find(id);
            if (order != null && order.Status == 0)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Update order status
                        order.Status = 1; // Changed to status 1 (Chưa thanh toán)
                        
                        // Update product quantities
                        var orderDetails = db.OrderDetails.Where(od => od.OrderId == id).ToList();
                        foreach (var detail in orderDetails)
                        {
                            var product = db.Products.Find(detail.ProductId);
                            if (product != null)
                            {
                                if (product.Quantity >= detail.Quanlity)
                                {
                                    product.Quantity -= detail.Quanlity;
                                }
                                else
                                {
                                    throw new Exception($"Sản phẩm {product.Title} không đủ số lượng trong kho");
                                }
                            }
                        }

                        db.SaveChanges();
                        transaction.Commit();

                        // Send confirmation email with order details
                        if (!string.IsNullOrEmpty(order.Email))
                        {
                            var strSanPham = "";
                            var thanhtien = decimal.Zero;
                            var TongTien = decimal.Zero;

                            foreach (var item in orderDetails)
                            {
                                var product = db.Products.Find(item.ProductId);
                                strSanPham += "<tr>";
                                strSanPham += "<td>" + product.Title + "</td>";
                                strSanPham += "<td>" + item.Quanlity + "</td>";
                                strSanPham += "<td>" + WebBanHang.Models.Common.Common.FormatNumber(item.Price, 0) + "</td>";
                                strSanPham += "</tr>";
                                thanhtien += item.Price * item.Quanlity;
                            }
                            TongTien = thanhtien;

                            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                            contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                            contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                            contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                            contentCustomer = contentCustomer.Replace("{{Email}}", order.Email);
                            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", WebBanHang.Models.Common.Common.FormatNumber(thanhtien, 0));
                            contentCustomer = contentCustomer.Replace("{{TongTien}}", WebBanHang.Models.Common.Common.FormatNumber(TongTien, 0));

                            WebBanHang.Models.Common.Common.SendMail("Web Bán Hàng", "Đơn hàng #" + order.Code + " đã được xác nhận", contentCustomer.ToString(), order.Email);

                            // Gửi email cho admin
                            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                            contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                            contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                            contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                            contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                            contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                            contentAdmin = contentAdmin.Replace("{{Email}}", order.Email);
                            contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                            contentAdmin = contentAdmin.Replace("{{ThanhTien}}", WebBanHang.Models.Common.Common.FormatNumber(thanhtien, 0));
                            contentAdmin = contentAdmin.Replace("{{TongTien}}", WebBanHang.Models.Common.Common.FormatNumber(TongTien, 0));

                            WebBanHang.Models.Common.Common.SendMail("Web Bán Hàng", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                        }

                        return Json(new { success = true, message = "Đơn hàng đã được xác nhận" });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }
            return Json(new { success = false, message = "Không tìm thấy đơn hàng hoặc đơn hàng đã được xác nhận" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}