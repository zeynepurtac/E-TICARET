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


namespace E_Commerce.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ECommerceContext _context;

        public CustomersController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
        public IActionResult Login(string currentUrl)
        {
            ViewData["currentUrl"] = currentUrl;
            return View();
        }
        public void TransferCard(long customerId, HttpContext httpContext, Models.ECommerceContext eCommerceContext, string? newCart = null)
        {

            byte i = 0;
            long productId;
            string? cart;
            if (newCart == null)
            {
                cart = Request.Cookies["cart"];
            }
            else
            {
                cart = newCart;
            }
            if (cart == "")
            {
                cart = null;
            }
            string[] cartItems, itemDetails;
            string cartItem;
            CookieOptions cookieOptions = new CookieOptions();
            OrderDetail orderDetail;
            Models.Product product;
            Order order;
            if (httpContext.Session.GetString("order") == null)
            {
                order = new Order();
                order.AllDelivered = false;
                order.IsCart = true;
                order.Cancelled = false;
                order.CustomerId = customerId;
                order.PaymentComplate = false;
                order.TimeStamp = DateTime.Now;
            }
            else
            {
                order = eCommerceContext.Orders.FirstOrDefault(o => o.OrderId == Convert.ToInt64(httpContext.Session.GetString("order")));
            }
            order.OrderDetails = new List<OrderDetail>();
            order.OrderPrice = 0;

            if (cart != null)
            {
                cartItems = cart.Split(",");
                for (i = 0; i < cartItems.Length; i++)
                {
                    orderDetail = new OrderDetail();
                    cartItem = cartItems[i];
                    itemDetails = cartItem.Split(":");

                    productId = Convert.ToInt32(itemDetails[0]);
                    product = eCommerceContext.Products.FirstOrDefault(m => m.ProductId == productId);

                    orderDetail.Count = Convert.ToByte(itemDetails[1]);
                    orderDetail.Price = product.ProductPrice * orderDetail.Count;
                    orderDetail.Product = product;
                    order.OrderPrice += orderDetail.Price;

                    order.OrderDetails.Add(orderDetail);

                }
                if (httpContext.Session.GetString("order") == null)
                {
                    eCommerceContext.Add(order);
                    eCommerceContext.SaveChanges();
                    if (order.OrderId != 0)
                    {
                        httpContext.Session.SetString("order", order.OrderId.ToString());
                    }
                }
                else
                {
                    eCommerceContext.Update(order);
                    eCommerceContext.SaveChanges();
                }

            }
            else
            {
                if (httpContext.Session.GetString("order") != null)
                {
                    eCommerceContext.Remove(order);
                    eCommerceContext.SaveChanges();
                    httpContext.Session.Remove("order");

                }
            }
        }
        // GET: Customers/Create
        public void ProcessLogin([Bind("CustomerEmail", "CustomerPassword")] Models.Customer customer, string currentUrl)
        {
            var dbUser = _context.Customers.FirstOrDefault(m => m.CustomerEmail == customer.CustomerEmail);
            SHA256 sHA256;
            byte[] hashedPassword;
            byte[] customerPassword;
            if (dbUser != null)
            {
                string controlpass;
                sHA256 = SHA256.Create();
                customerPassword = Encoding.Unicode.GetBytes(customer.CustomerEmail.Trim() + customer.CustomerPassword.Trim());
                hashedPassword = sHA256.ComputeHash(customerPassword);
                controlpass = BitConverter.ToString(hashedPassword).Replace("-", "");
                if (controlpass == dbUser.CustomerPassword)
                {
                    this.HttpContext.Session.SetString("customer", dbUser.CustomerId.ToString());
                    TransferCard(dbUser.CustomerId, this.HttpContext, _context);
                    Response.Redirect(currentUrl);
                    return;
                }
            }
            Response.Redirect("/Login");
        }
        public IActionResult Create(string currentUrl, bool noPassword = false)
        {
            ViewData["noPassword"] = noPassword;
            ViewData["currentUrl"] = currentUrl;
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,CustomerSurname,CustomerEmail,CustomerPhone,CustomerPassword,CustomerConfirmPassword,CustomerAdress,IsDeleted")] Customer customer, string currentUrl)
        {

            if (ModelState.IsValid)
            {
                SHA256 sHA256;
                byte[] hashedPassword;
                byte[] customerPassword;
                string controlpass;
                sHA256 = SHA256.Create();
                customerPassword = Encoding.Unicode.GetBytes(customer.CustomerEmail.Trim() + customer.CustomerPassword.Trim());
                hashedPassword = sHA256.ComputeHash(customerPassword);
                controlpass = BitConverter.ToString(hashedPassword).Replace("-", "");
                customer.CustomerPassword = controlpass;


                _context.Add(customer);
                await _context.SaveChangesAsync();
                this.HttpContext.Session.SetString("customer", customer.CustomerId.ToString());
                TransferCard(customer.CustomerId, this.HttpContext, _context);
                return Redirect(currentUrl);

            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("CustomerId,CustomerName,CustomerSurname,CustomerEmail,CustomerPhone,CustomerPassword,CustomerAdress,IsDeleted")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ECommerceContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(long id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
