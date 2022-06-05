using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Areas.Admin.Models;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserContext _context;
        public HomeController(UserContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login([Bind("UserEMail", "UserPassword")] User user)
        {
            var dbUser = _context.Users.FirstOrDefault(m => m.UserEMail == user.UserEMail);
            SHA256 sHA256;
            byte[] hashedPassword;                                                                                                       
            byte[] userPassword;
           

            if (dbUser != null)
            {
                string controlpass;
                sHA256 = SHA256.Create();
                userPassword = Encoding.Unicode.GetBytes(user.UserEMail.Trim() + user.UserPassword.Trim());
                hashedPassword = sHA256.ComputeHash(userPassword);
                controlpass = BitConverter.ToString(hashedPassword).Replace("-", "");

                if (controlpass == dbUser.UserPassword)
                { 
                    // kişi login olduğunda sadece kullkanıcıyı bilmek yetmiyor, yetkilerini de sessionın içinde kalmalı.
                    // sessionda kullanıcı ıdsi ve yetkisi tutuluyor.
                    //loginde sessiona bütün yetkilere bakmak gerekiyor . hangi yetki verildiyse kontrollerda bakmak lazım .
                    this.HttpContext.Session.SetString("guest", dbUser.UserId.ToString());  //key and value olarak tutulur. (guest=1) 

                    this.HttpContext.Session.SetString("viewUsers", dbUser.ViewUsers.ToString());
                    this.HttpContext.Session.SetString("createUsers", dbUser.CreateUser.ToString());
                    this.HttpContext.Session.SetString("deleteUsers", dbUser.DeleteUser.ToString());
                    this.HttpContext.Session.SetString("editUsers", dbUser.EditUser.ToString());

                    this.HttpContext.Session.SetString("viewSellers", dbUser.ViewSellers.ToString());
                    this.HttpContext.Session.SetString("createSellers", dbUser.CreateSeller.ToString());
                    this.HttpContext.Session.SetString("deleteSellers", dbUser.DeleteSeller.ToString());
                    this.HttpContext.Session.SetString("editSellers", dbUser.EditSeller.ToString());

                    this.HttpContext.Session.SetString("viewCategories", dbUser.ViewCategories.ToString());
                    this.HttpContext.Session.SetString("createCategory", dbUser.CreateCategory.ToString());
                    this.HttpContext.Session.SetString("deleteCategory", dbUser.DeleteCategory.ToString());
                    this.HttpContext.Session.SetString("editCategory", dbUser.EditCategory.ToString());


                    this.HttpContext.Session.SetString("deleteProduct", dbUser.DeleteProduct.ToString());
                    this.HttpContext.Session.SetString("editProduct", dbUser.EditCProduct.ToString());


                    return RedirectToAction("Index", "Users");
                    //return RedirectToAction(nameof(Login));
                    //Response.Redirect("~/Admin/Users/Index");
                }

            }


            return RedirectToAction("Index");  // login olamadıysa tekrar başa sarıyor. 
        }
    }
}
