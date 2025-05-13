using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            // Nếu không chỉ định Roles, cho phép tất cả người dùng đã đăng nhập
            if (string.IsNullOrEmpty(Roles))
            {
                return true;
            }

            // Kiểm tra vai trò của người dùng
            var allowedRoles = Roles.Split(',').Select(r => r.Trim()).ToList();
            foreach (var role in allowedRoles)
            {
                if (httpContext.User.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // Trả về JsonResult cho Ajax
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = "Bạn không có quyền thực hiện hành động này." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.HttpContext.Response.StatusCode = 403; // Forbidden
            }
            else
            {
                // Lưu thông báo lỗi vào ViewBag cho non-Ajax
                filterContext.Controller.ViewBag.AccessDeniedMessage = "Bạn không có quyền thực hiện hành động này.";
                // Render view hiện tại để giữ giao diện
                filterContext.Result = new ViewResult
                {
                    ViewName = filterContext.RouteData.Values["action"].ToString()
                };
            }
        }
    }
}