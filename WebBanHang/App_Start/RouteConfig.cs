﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebBanHang
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Contact",
                url: "lien-he",
                defaults: new { controller = "Contact", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );
            routes.MapRoute(
               name: "vnpay_return",
               url: "vnpay_return",
               defaults: new { controller = "ShoppingCart", action = "VnpayReturn", alias = UrlParameter.Optional },
               namespaces: new[] { "WebBanHang.Controllers" }
           );
            routes.MapRoute(
                name: "BaiViet",
                url: "post/{alias}",
                defaults: new { controller = "Article", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );
            routes.MapRoute(
                name: "ShoppingCart",
                url: "gio-hang",
                defaults: new { controller = "ShoppingCart", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );
            routes.MapRoute(
               name: "detailProduct",
               url: "chi-tiet/{alias}-p{id}",
               defaults: new { controller = "Products", action = "Detail", alias = UrlParameter.Optional },
               namespaces: new[] { "WebBanHang.Controllers" }
           );
            routes.MapRoute(
               name: "detailNews",
               url: "bai-viet/{alias}-n{id}",
               defaults: new { controller = "News", action = "Detail", alias = UrlParameter.Optional },
               namespaces: new[] { "WebBanHang.Controllers" }
           );
            routes.MapRoute(
                name: "Products",
                url: "san-pham",
                defaults: new { controller = "Products", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );
            routes.MapRoute(
                name: "Tin Tức",
                url: "tin-tuc",
                defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );
            routes.MapRoute(
                  name: "CheckOut",
                  url: "thanh-toan",
                  defaults: new { controller = "ShoppingCart", action = "CheckOut", alias = UrlParameter.Optional },
                  namespaces: new[] { "WebBanHang.Controllers" }
             );
            routes.MapRoute(
                name: "CategoryProduct",
                url: "danh-muc-san-pham/{alias}-{id}",
                defaults: new { controller = "Products", action = "ProductCategory", id = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );

            routes.MapRoute(
                name: "AboutUs",
                url: "ve-chung-toi",
                defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebBanHang.Controllers" }
            );
        }
    }
}
