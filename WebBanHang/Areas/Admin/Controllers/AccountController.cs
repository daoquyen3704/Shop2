using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHang;
using WebBanHang.Models;
using WebBanHang.Areas.Admin.Filters;

namespace WebBanHang.Areas.Admin.Controllers
{
    
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/Account
        [CustomAuthorize(Roles = "Admin,Employee")]
        public ActionResult Index()
        {
            var items = db.Users.ToList();
            var roles = db.Roles.ToList();
            var userRoles = new Dictionary<string, List<string>>();
            
            foreach (var user in items)
            {
                var userRoleIds = db.Set<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole>()
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .ToList();
                
                var userRoleNames = roles
                    .Where(r => userRoleIds.Contains(r.Id))
                    .Select(r => r.Name)
                    .ToList();
                
                userRoles[user.Id] = userRoleNames;
            }
            
            ViewBag.Roles = roles;
            ViewBag.UserRoles = userRoles;
            return View(items);
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // Nếu user đã đăng nhập và là Customer, chuyển về trang chủ
            if (User.Identity.IsAuthenticated && User.IsInRole("Customer"))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra xác thực
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindByNameAsync(model.UserName);
                    if (user != null && await UserManager.IsInRoleAsync(user.Id, "Customer"))
                    {
                        // Nếu là Customer, luôn chuyển về trang chủ
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Account/Register

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Id", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Tìm role theo ID
                    var role = db.Roles.FirstOrDefault(r => r.Id == model.Role);
                    if (role != null)
                    {
                        // Sử dụng tên role thay vì ID
                        var roleResult = await UserManager.AddToRoleAsync(user.Id, role.Name);
                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            await UserManager.DeleteAsync(user);
                            ViewBag.Role = new SelectList(db.Roles.ToList(), "Id", "Name");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Role không tồn tại");
                        await UserManager.DeleteAsync(user);
                        ViewBag.Role = new SelectList(db.Roles.ToList(), "Id", "Name");
                        return View(model);
                    }

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Id", "Name");
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            var item = db.Users.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Id", "Name");
            return View(item);
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Phone = model.Phone;

                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    AddErrors(result);
                }
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Id", "Name");
            var userInDb = await UserManager.FindByIdAsync(model.Id);
            if (userInDb == null)
            {
                // THÊM LOG TẠM: Báo lỗi ra cho dễ trace
                ModelState.AddModelError("", "Không tìm thấy user để edit.");
                return RedirectToAction("Index");
            }

            return View(userInDb);
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không thể xóa tài khoản" });
            }
            return Json(new { success = false, message = "Tài khoản không tồn tại" });
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ChangePassword(string id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = id;
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "Mật khẩu không được để trống");
                return View();
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Xóa mật khẩu cũ
                await UserManager.RemovePasswordAsync(userId);
                // Thêm mật khẩu mới
                var result = await UserManager.AddPasswordAsync(userId, newPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}