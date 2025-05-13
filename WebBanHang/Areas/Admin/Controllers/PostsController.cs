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
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Posts
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            var pageIndex = page ?? 1;

            IQueryable<Posts> query = db.Posts.OrderByDescending(x => x.Id);

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
        public ActionResult Add(Posts model)
        {
            if (ModelState.IsValid)
            {

                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                db.Posts.Add(model);
                model.Alias = WebBanHang.Models.Common.Filter.FilterChar(model.Title);
                model.CategoryId = 1;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var items = db.Posts.Find(id);
            if (items == null)
            {
                return RedirectToAction("Index");
            }
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Posts model)
        {
            if (ModelState.IsValid)
            {

                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebBanHang.Models.Common.Filter.FilterChar(model.Title);
                db.Posts.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                model.CategoryId = 1;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Posts.Find(id);
            if (item != null)
            {
                db.Posts.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Posts.Find(id);
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
                var objectsToRemove = db.Posts.Where(n => items.Contains(n.Id)).ToList();

                if (objectsToRemove.Any())
                {
                    db.Posts.RemoveRange(objectsToRemove);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
    }
}