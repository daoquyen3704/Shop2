using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.EF;
using System.Globalization;
using System.Text;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/News
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            var pageIndex = page ?? 1;

            IQueryable<News> query = db.News.OrderByDescending(x => x.Id);

            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchTerm = Searchtext.Trim().ToLower();
                query = query.Where(x =>
                    x.Alias.ToLower().Contains(searchTerm) ||
                    x.Title.ToLower().Contains(searchTerm)
                );
            }

            var pagedItems = query.ToPagedList(pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageIndex;

            return View(pagedItems);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(News model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.UtcNow.AddHours(7); // giờ Việt Nam
                model.ModifiedDate = DateTime.UtcNow.AddHours(7); // nếu cần
                model.Alias = WebBanHang.Models.Common.Filter.FilterChar(model.Title);
                model.CategoryId = 1;

                db.News.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var items = db.News.Find(id);
            if (items == null)
            {
                return RedirectToAction("Index");
            }
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(News model)
        {
            if (ModelState.IsValid)
            {
                var item = db.News.Find(model.Id);
                if (item == null)
                {
                    return HttpNotFound();
                }

                item.Title = model.Title;
                item.Image = model.Image;
                item.Description = model.Description;
                item.Detail = model.Detail;
                item.IsActive = model.IsActive;
                item.SeoTitle = model.SeoTitle;
                item.SeoDescription = model.SeoDescription;
                item.SeoKeywords = model.SeoKeywords;
                item.ModifiedDate = DateTime.UtcNow.AddHours(7); // cập nhật ngày sửa

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.News.Find(id);
            if (item != null)
            {
                db.News.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.News.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsActive = item.IsActive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',').Select(id => Convert.ToInt32(id)).ToList();
                var objectsToRemove = db.News.Where(n => items.Contains(n.Id)).ToList();

                if (objectsToRemove.Any())
                {
                    db.News.RemoveRange(objectsToRemove);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
    }
}
