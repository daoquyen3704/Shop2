using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.EF;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ProductImageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ProductImage
        public ActionResult Index(int id)
        {
            ViewBag.ProductId = id;
            var items = db.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(items);
        }

        [HttpPost]
        public ActionResult AddImage(int productId, string url)
        {
            db.ProductImages.Add(new ProductImage
            {
                ProductId = productId,
                Image = url,
                IsDefault = false
            });
            db.SaveChanges();
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.ProductImages.Find(id);
            if (item == null)
            {
                return Json(new { success = false, message = "Ảnh không tồn tại!" });
            }

            bool isDefault = item.IsDefault;
            int productId = item.ProductId;

            db.ProductImages.Remove(item);
            db.SaveChanges();

            if (isDefault)
            {
                var newDefaultImage = db.ProductImages.Where(x => x.ProductId == productId).FirstOrDefault();
                if (newDefaultImage != null)
                {
                    newDefaultImage.IsDefault = true;
                    db.SaveChanges();
                }
            }

            return Json(new { success = true });
        }


        //[HttpPost]
        //public ActionResult Delete(int id)
        //{
        //    var item = db.ProductImages.Find(id);
        //    db.ProductImages.Remove(item);
        //    db.SaveChanges();
        //    return Json(new { success = true });
        //}

        [HttpPost]
        public ActionResult SetDefault(int id)
        {
            var image = db.ProductImages.FirstOrDefault(x => x.Id == id);
            if (image == null)
            {
                return Json(new { success = false, message = "Ảnh không tồn tại!" });
            }

            // Bỏ Default của ảnh khác cùng ProductId
            db.ProductImages.Where(x => x.ProductId == image.ProductId).ToList().ForEach(x => x.IsDefault = false);

            // Set ảnh này là Default
            image.IsDefault = true;
            db.SaveChanges();

            return Json(new { success = true });
        }


    }
}