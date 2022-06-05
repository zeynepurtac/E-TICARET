using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;

namespace E_Commerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ECommerceContext _context;

        public HomeController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: Home
        public async Task<IActionResult> Index()
        {
            var eCommerceContext = _context.Products.Where(p=>p.IsDeleted==false).Include(p => p.Brand).Include(p => p.Category).Include(p => p.Seller);
            return View(await eCommerceContext.ToListAsync());
        }
        public async Task<IActionResult> JsonIndex()  // androidde json formatında döner
        {

            return Json(await _context.Products.ToListAsync()); ;
        }
        // GET: Home/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public async Task<IActionResult> JsonDetails()  // androidde json formatında döner
        {

            return Json(await _context.Products.ToListAsync()); ;
        }

        // GET: Home/Create
    }
}
