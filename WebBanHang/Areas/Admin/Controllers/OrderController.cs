using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Areas.Admin.Filters;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index(int? page)
        {
            var items = db.Orders.OrderByDescending(x => x.CreatedDate).ToList();

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

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Cancel(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null || order.Status == 4 || order.Status == 3)
            {
                return HttpNotFound();
            }

            // Cập nhật trạng thái thành "Hủy" (Status = 4)
            order.Status = 4;
            db.SaveChanges();

            // Gửi email thông báo hủy
            if (!string.IsNullOrEmpty(order.Email))
            {
                try
                {
                    var mail = new MailMessage();
                    mail.From = new MailAddress("your-email@gmail.com");
                    mail.To.Add(order.Email);
                    mail.Subject = $"Thông báo hủy đơn hàng #{order.Code}";
                    mail.Body = $@"
                        Kính gửi {order.CustomerName},
                        
                        Đơn hàng của bạn với mã #{order.Code} đã bị hủy.
                        Thông tin đơn hàng:
                        - Mã đơn hàng: {order.Code}
                        - Tổng tiền: {order.TotalAmount:N0} VNĐ
                        - Ngày đặt: {order.CreatedDate:dd/MM/yyyy HH:mm}
                        
                        Nếu bạn có thắc mắc, vui lòng liên hệ chúng tôi qua email hoặc số điện thoại.
                        
                        Trân trọng,
                        Đội ngũ WebBanHang
                    ";
                    mail.IsBodyHtml = false;

                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(mail);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi gửi email: {ex.Message}");
                    TempData["ErrorMessage"] = "Đã hủy đơn hàng, nhưng không thể gửi email thông báo.";
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                item.Status = trangthai;
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
    }
}