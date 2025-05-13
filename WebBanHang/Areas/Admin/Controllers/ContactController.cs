using ClosedXML.Excel; // Thêm namespace cho ClosedXML
using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.EF;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ContactController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Contact
        public ActionResult Index()
        {
            var contacts = db.Contacts.OrderByDescending(c => c.CreatedDate).ToList();
            return View(contacts);
        }

        public ActionResult Detail(int id)
        {
            var contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            // Mark as read when viewing details
            if (!contact.IsRead)
            {
                contact.IsRead = true;
                db.SaveChanges();
            }

            return View(contact);
        }

        // POST: Admin/Contact/SendEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmail(int id, string subject, string message)
        {
            var contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            try
            {
                // Lấy thông tin từ Web.config
                string fromEmail = ConfigurationManager.AppSettings["Email"];
                string emailPassword = ConfigurationManager.AppSettings["PasswordEmail"];

                // Cấu hình SMTP
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, emailPassword),
                    EnableSsl = true,
                };

                // Tạo email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Luxury Watch"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(contact.Email);

                // Gửi email
                smtpClient.Send(mailMessage);

                TempData["Message"] = "Gửi email thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi gửi email: {ex.Message}";
            }

            return RedirectToAction("Detail", new { id = id });
        }

        // GET: Admin/Contact/ExportToExcel
        public ActionResult ExportToExcel()
        {
            var contacts = db.Contacts.OrderByDescending(c => c.CreatedDate).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("LienHe");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Tên";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Website";
                worksheet.Cell(1, 5).Value = "Lời nhắn";
                worksheet.Cell(1, 6).Value = "Ngày gửi";
                worksheet.Cell(1, 7).Value = "Trạng thái";

                // Dữ liệu
                for (int i = 0; i < contacts.Count; i++)
                {
                    var contact = contacts[i];
                    worksheet.Cell(i + 2, 1).Value = contact.Id;
                    worksheet.Cell(i + 2, 2).Value = contact.Name;
                    worksheet.Cell(i + 2, 3).Value = contact.Email;
                    worksheet.Cell(i + 2, 4).Value = contact.Website;
                    worksheet.Cell(i + 2, 5).Value = contact.Message;
                    worksheet.Cell(i + 2, 6).Value = contact.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
                    worksheet.Cell(i + 2, 7).Value = contact.IsRead ? "Đã đọc" : "Chưa đọc";
                }

                // Định dạng
                worksheet.Columns().AdjustToContents();
                worksheet.Range(1, 1, 1, 7).Style.Font.Bold = true;

                // Tạo MemoryStream mà không dùng "using"
                var stream = new System.IO.MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"DanhSachLienHe_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        // POST: Admin/Contact/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Không tìm thấy liên hệ" });
            }

            db.Contacts.Remove(contact);
            db.SaveChanges();
            return Json(new { success = true, message = "Xóa liên hệ thành công" });
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