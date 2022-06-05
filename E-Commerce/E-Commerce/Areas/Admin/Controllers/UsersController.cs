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
    public class UsersController : Controller
    {
        private readonly UserContext _context;
        AuthorizationClass authorization = new AuthorizationClass(); //hem login olmuş olması lazım hemde yetkisi olması lazım 

        public UsersController(UserContext context)
        {

            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {  // kullanıcının çalıştırabilmesi için bu yetkiye sahip olması gerekmekte 
           //yetkiyi kontrol etmek gerekmekte authorizationda yetkiye bakmak gerekiyor.

            if (authorization.IsAuthorized("viewUsers", this.HttpContext.Session) == false)
            {
                return Problem("You do not have authorization to view this page.");    // boş döndür ya da hatayı söyle
            }
            return _context.Users != null ?
                        View(await _context.Users.Where(u => u.IsDeleted == false).ToListAsync()) : //deleted false olanlar gelsin / listeye silinenler gelmesin diye indextende sildik
                        Problem("Entity set 'UserContext.Users'  is null.");
        }
        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        // GET: Admin/Users/Create
        public IActionResult Create()
        {

            ////if (authorization.IsAuthorized("createUsers", this.HttpContext.Session) == false)
            ////{
            ////    return Problem("Yetkin yok."); // boş döndür ya da hatayı söyle
            ////}
            return View();
        }
        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Name,UserEMail,UserPassword,ConfirmPassword,IsDeleted,ViewUsers,CreateUser,DeleteUser,EditUser,ViewSellers,CreateSeller,DeleteSeller,EditSeller,ViewCategories,CreateCategory,DeleteCategory,EditCategory,DeleteProduct,EditCProduct")] User user)
        {
            SHA256 sHA256;
            byte[] hashedPassword;
            byte[] userPassword;
            //if (authorization.IsAuthorized("createUsers", this.HttpContext.Session) == false)
            //{
            //    return Problem("Yetkin yok."); ;
            //}
            if (ModelState.IsValid)
            {
                sHA256 = SHA256.Create();
                userPassword = Encoding.Unicode.GetBytes(user.UserEMail.Trim() + user.UserPassword.Trim());
                hashedPassword = sHA256.ComputeHash(userPassword);
                user.UserPassword = BitConverter.ToString(hashedPassword).Replace("-", "");
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (authorization.IsAuthorized("editUsers", this.HttpContext.Session) == false)
            {
                return Problem("Yetkin yok.");  // boş döndür ya da hatayı söyle
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("UserId,Name,UserEMail,UserPassword,ConfirmPassword,IsDeleted,ViewUsers,CreateUser,DeleteUser,EditUser,ViewSellers,CreateSeller,DeleteSeller,EditSeller,ViewCategories,CreateCategory,DeleteCategory,EditCategory,DeleteProduct,EditCProduct")] User user, string OldPassword, string OriginalPassword)
        {
            SHA256 sHA256;
            byte[] hashedPassword, userPassword;
            string oldHash;   // eski hashi tutuyoruz

            if (authorization.IsAuthorized("editUsers", this.HttpContext.Session) == false)
            {
                return Problem("Yetkin yok.");  // boş döndür ya da hatayı söyle
            }
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                sHA256 = SHA256.Create();
                userPassword = Encoding.Unicode.GetBytes(user.UserEMail.Trim() + OldPassword.Trim());
                hashedPassword = sHA256.ComputeHash(userPassword);
                oldHash = BitConverter.ToString(hashedPassword).Replace("-", "");
                if (oldHash == OriginalPassword)
                {
                    userPassword = Encoding.Unicode.GetBytes(user.UserEMail.Trim() + user.UserPassword.Trim());
                    hashedPassword = sHA256.ComputeHash(userPassword);
                    user.UserPassword = BitConverter.ToString(hashedPassword).Replace("-", "");
                    try
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.UserId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                }
                return RedirectToAction(nameof(Index));

            }

            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (authorization.IsAuthorized("deleteUsers", this.HttpContext.Session) == false)
            {
                return Problem("Yetkin yok.");   // boş döndür ya da hatayı söyle
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();

            }
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            if (authorization.IsAuthorized("deleteUsers", this.HttpContext.Session) == false)
            {
                return null;   // boş döndür ya da hatayı söyle
            }

            if (_context.Users == null)
            {
                return Problem("Entity set 'UserContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {

                user.IsDeleted = true;

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(short id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
