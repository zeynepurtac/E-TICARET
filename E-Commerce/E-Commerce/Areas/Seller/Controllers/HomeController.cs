using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class HomeController : Controller
    {
        private readonly ECommerceContext _context;

        public HomeController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: Seller/Home
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Login([Bind("SellerEMail", "SellerPassword")] Models.Seller seller)
        {
            var dbUser = _context.Sellers.FirstOrDefault(m => m.SellerEMail == seller.SellerEMail);
            SHA256 sHA256;
            byte[] hashedPassword;
            byte[] sellerPassword;
            if (dbUser != null)
            {
                string controlpass;
                sHA256 = SHA256.Create();
                sellerPassword = Encoding.Unicode.GetBytes(seller.SellerEMail.Trim() + seller.SellerPassword.Trim());
                hashedPassword = sHA256.ComputeHash(sellerPassword);
                controlpass = BitConverter.ToString(hashedPassword).Replace("-", "");
                if (controlpass == dbUser.SellerPassword)
                {
                    this.HttpContext.Session.SetInt32("merchant",dbUser.SellerId);
                    return RedirectToAction("Index", "Products");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
