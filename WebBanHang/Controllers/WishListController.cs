using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.EF;

namespace WebBanHang.Controllers
{
    [Authorize]
    public class WishListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/WishList
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<WishList> items = db.WishLists.Where(x => x.Username == User.Identity.Name).OrderByDescending(x => x.CreatedDate);
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult PostWishlist(int ProductId)
        {
            if (Request.IsAuthenticated == false)
            {
                return Json(new { Success = false, Message = "Bạn chưa đăng nhập." });
            }
            var checkItem = db.WishLists.FirstOrDefault(x => x.ProductId == ProductId && x.Username == User.Identity.Name);
            if (checkItem != null)
            {
                return Json(new { Success = false, IsExists = true });
            }
            var item = new WishList();
            item.ProductId = ProductId;
            item.Username = User.Identity.Name;
            item.CreatedDate = DateTime.Now;
            db.WishLists.Add(item);
            db.SaveChanges();
            return Json(new { Success = true, IsExists = false });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult PostDeleteWishlist(int ProductId)
        {
            var checkItem = db.WishLists.FirstOrDefault(x => x.ProductId == ProductId && x.Username == User.Identity.Name);
            if (checkItem != null)
            {
                var item = db.WishLists.Find(checkItem.Id);
                db.Set<WishList>().Remove(item);
                var i = db.SaveChanges();
                return Json(new { Success = true, Message = "Xóa thành công." });
            }
            return Json(new { Success = false, Message = "Xóa thất bại." });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}