using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using WebBanHang.Models.EF;
using System.Globalization;
using System.Text;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;
using WebBanHang.Areas.Admin.Filters;

namespace WebBanHang.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,Employee")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public DbSet<ProductImage> ProductImage { get; set; }

        // GET: Admin/Products
        public ActionResult Index(string Searchtext, int? page)
        {
            int pageSize = 5;
            var pageIndex = page ?? 1;
            IQueryable<Product> query = db.Products.OrderByDescending(x => x.Id);
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


        
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images, List<int> rDefault)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Thiết lập các giá trị mặc định
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.ViewCount = 0; // Thêm giá trị mặc định cho ViewCount
                    model.IsActive = true; // Mặc định sản phẩm được kích hoạt

                    // Xử lý SEO
                    if (string.IsNullOrEmpty(model.SeoTitle))
                    {
                        model.SeoTitle = model.Title;
                    }
                    if (string.IsNullOrEmpty(model.Alias))
                    {
                        model.Alias = WebBanHang.Models.Common.Filter.FilterChar(model.Title);
                    }

                    // Xử lý ảnh sản phẩm
                    if (Images != null && Images.Count > 0)
                    {
                        // Đảm bảo model.ProductImage được khởi tạo
                        if (model.ProductImage == null)
                        {
                            model.ProductImage = new List<ProductImage>();
                        }

                        // Xử lý ảnh mặc định
                        if (rDefault != null && rDefault.Count > 0)
                        {
                            int defaultIndex = rDefault[0] - 1;
                            if (defaultIndex >= 0 && defaultIndex < Images.Count)
                            {
                                model.Image = Images[defaultIndex];
                            }
                            else
                            {
                                model.Image = Images[0];
                            }
                        }
                        else
                        {
                            model.Image = Images[0];
                        }

                        // Thêm các ảnh vào ProductImage
                        for (int i = 0; i < Images.Count; i++)
                        {
                            bool isDefault = (rDefault != null && rDefault.Count > 0 && i == rDefault[0] - 1);
                            model.ProductImage.Add(new ProductImage
                            {
                                Image = Images[i],
                                IsDefault = isDefault
                            });
                        }
                    }

                    // Lưu sản phẩm vào database
                    db.Products.Add(model);
                    db.SaveChanges();

                    // Cập nhật ProductId cho các ProductImage
                    if (model.ProductImage != null && model.ProductImage.Count > 0)
                    {
                        foreach (var item in model.ProductImage)
                        {
                            item.ProductId = model.Id;
                        }
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    // In ra lỗi validation để debug
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Lỗi ở field {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi
                System.Diagnostics.Debug.WriteLine("Lỗi: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title");
            return View(model);
        }

        
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title", product.ProductCategoryId);
            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, List<string> Images, List<int> rDefault)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = db.Products.Find(model.Id);
                    if (existingProduct == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật thông tin sản phẩm
                    existingProduct.Title = model.Title;
                    existingProduct.Alias = WebBanHang.Models.Common.Filter.FilterChar(model.Title);
                    existingProduct.ProductCode = model.ProductCode;
                    existingProduct.ProductCategoryId = model.ProductCategoryId;
                    existingProduct.Description = model.Description;
                    existingProduct.Detail = model.Detail;
                    existingProduct.Quantity = model.Quantity > 0 ? model.Quantity : existingProduct.Quantity; // Giữ nguyên số lượng cũ nếu số lượng mới <= 0
                    existingProduct.Price = model.Price;
                    existingProduct.PriceSale = model.PriceSale;
                    existingProduct.OriginalPrice = model.OriginalPrice;
                    existingProduct.SeoTitle = model.SeoTitle;
                    existingProduct.SeoKeywords = model.SeoKeywords;
                    existingProduct.SeoDescription = model.SeoDescription;
                    existingProduct.IsActive = model.IsActive;
                    existingProduct.IsHot = model.IsHot;
                    existingProduct.IsFeature = model.IsFeature;
                    existingProduct.IsSale = model.IsSale;
                    existingProduct.ModifiedDate = DateTime.Now;

                    // Xử lý hình ảnh
                    if (Images != null && Images.Count > 0)
                    {
                        // Xóa các hình ảnh cũ
                        var oldImages = db.Set<ProductImage>().Where(x => x.ProductId == model.Id).ToList();
                        if (oldImages != null && oldImages.Count > 0)
                        {
                            db.Set<ProductImage>().RemoveRange(oldImages);
                        }

                        // Thêm hình ảnh mới
                        for (int i = 0; i < Images.Count; i++)
                        {
                            bool isDefault = (rDefault != null && rDefault.Count > 0 && i == rDefault[0] - 1);
                            var productImage = new ProductImage
                            {
                                ProductId = model.Id,
                                Image = Images[i],
                                IsDefault = isDefault
                            };
                            db.Set<ProductImage>().Add(productImage);

                            // Cập nhật hình ảnh đại diện nếu là ảnh mặc định
                            if (isDefault)
                            {
                                existingProduct.Image = Images[i];
                            }
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            ViewBag.ProductCategory = new SelectList(db.ProductCategories.ToList(), "Id", "Title", model.ProductCategoryId);
            return View(model);
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult ToggleStatus(int id, string field)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return Json(new { success = false });
            }

            switch (field)
            {
                case "IsHome":
                    product.IsHome = !product.IsHome;
                    break;
                case "IsSale":
                    product.IsSale = !product.IsSale;
                    break;
                case "IsActive":
                    product.IsActive = !product.IsActive;
                    break;
                default:
                    return Json(new { success = false });
            }

            db.SaveChanges();
            return Json(new { success = true, status = (bool)product.GetType().GetProperty(field).GetValue(product) });
        }


        
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var product = db.Products.Find(id);
                if (product != null)
                {
                    // Kiểm tra nếu db.ProductImage là object -> Sử dụng db.Set<ProductImage>()
                    var productImages = db.Set<ProductImage>().Where(x => x.ProductId == id).ToList();

                    foreach (var img in productImages)
                    {
                        db.Set<ProductImage>().Remove(img);
                    }

                    db.Products.Remove(product);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    var idList = ids.Split(',').Select(int.Parse).ToList();
                    var products = db.Products.Where(p => idList.Contains(p.Id)).ToList();

                    if (products.Any())
                    {
                        foreach (var product in products)
                        {
                            // Xóa ảnh sản phẩm nếu có
                            var productImages = db.Set<ProductImage>().Where(x => x.ProductId == product.Id).ToList();
                            if (productImages.Any())
                            {
                                foreach (var img in productImages)
                                {
                                    db.Set<ProductImage>().Remove(img);
                                }
                            }

                            // Xóa sản phẩm
                            db.Products.Remove(product);
                        }

                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false, message = "Không có sản phẩm nào để xóa." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        

        //[HttpPost]
        //public ActionResult DeleteAll(string ids)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(ids))
        //        {
        //            var items = ids.Split(',');
        //            if (items != null && items.Any())
        //            {
        //                foreach (var item in items)
        //                {
        //                    var id = int.Parse(item);
        //                    var product = db.Products.Find(id);
        //                    if (product != null)
        //                    {
        //                        // Xóa các ảnh liên quan
        //                        var productImages = db.ProductImage.Where(x => x.ProductId == id);
        //                        if (productImages != null && productImages.Count() > 0)
        //                        {
        //                            db.ProductImage.RemoveRange(productImages);
        //                        }
        //                        // Xóa sản phẩm
        //                        db.Products.Remove(product);
        //                    }
        //                }
        //                db.SaveChanges();
        //                return Json(new { success = true });
        //            }
        //        }
        //        return Json(new { success = false });
        //    }
        //    catch
        //    {
        //        return Json(new { success = false });
        //    }
        //}
    }
}