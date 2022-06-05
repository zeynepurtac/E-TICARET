using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class CartController : Controller
    {
        public struct CartProduct
        {
            public Models.Product Product;
            public int Count;
            public float Total;

        }
        public string Add(long id)
        {

            DbContextOptions<Models.ECommerceContext> options = new DbContextOptions<Models.ECommerceContext>();
            Models.ECommerceContext eCommerceContext = new Models.ECommerceContext(options);
            CustomersController customersController = new CustomersController(eCommerceContext);

            string cart = Request.Cookies["cart"];
            string[] cartItems;
            string[] itemDetails;
            short itemCount;
            string cartItem;
            string newCart = "";
            short totalCount = 0;
            bool itemExist = false;

            CookieOptions cookieOptions = new CookieOptions();
            if (cart == null)
            {
                newCart = id.ToString() + ":1";
                totalCount = 1;
            }
            else
            {
                cartItems = cart.Split(",");

                for (short i = 0; i < cartItems.Length; i++)
                {
                    cartItem = cartItems[i];
                    itemDetails = cartItem.Split(":");
                    itemCount = Convert.ToInt16(itemDetails[1]);

                    if (itemDetails[0] == id.ToString())
                    {

                        itemCount++;
                        itemExist = true;

                    }
                    totalCount += itemCount;
                    newCart = newCart + itemDetails[0] + ":" + itemCount.ToString();
                    if (i < cartItems.Length - 1)
                    {
                        newCart = newCart + ",";
                    }
                }
                if (itemExist == false)
                {
                    newCart = newCart + "," + id.ToString() + ":1";
                    totalCount++;
                }

            }
            cookieOptions.Path = "/";
            cookieOptions.Expires = DateTime.MaxValue;
            Response.Cookies.Append("cart", newCart, cookieOptions);
            if (this.HttpContext.Session.GetString("customer") != null)
            {
                customersController.TransferCard(Convert.ToInt64(this.HttpContext.Session.GetString("customer")),this.HttpContext,eCommerceContext,newCart);
            }
            return totalCount.ToString();

        }
        public IActionResult Index()
        {
            DbContextOptions<Models.ECommerceContext> options = new DbContextOptions<Models.ECommerceContext>();
            Models.ECommerceContext eCommerceContext = new Models.ECommerceContext(options);
            Areas.Seller.Controllers.ProductsController productsController = new Areas.Seller.Controllers.ProductsController(eCommerceContext);
            Models.Product product;
            byte i = 0;
            long productId;
            string? cart = Request.Cookies["cart"];
            string[] cartItems, itemDetails;
            string cartItem;
            float cartTotal = 0;
            List<CartProduct> cartProducts = new List<CartProduct>();
            if (cart != null)
            {
                cartItems = cart.Split(",");
                for (i = 0; i < cartItems.Length; i++)
                {
                    cartItem = cartItems[i];
                    itemDetails = cartItem.Split(":");
                    CartProduct cartProduct = new CartProduct();
                    productId = Convert.ToInt32(itemDetails[0]);
                    product = productsController.Product(productId);
                    cartProduct.Product = product;
                    cartProduct.Count = Convert.ToInt32(itemDetails[1]);
                    cartProduct.Total = cartProduct.Count * product.ProductPrice;
                    cartProducts.Add(cartProduct);

                    cartTotal += cartProduct.Total;
                }
            }

            ViewData["product"] = cartProducts;
            ViewData["cartTotal"] = cartTotal;
            return View();
        }
        public string CalculateTotal(long id, byte count)
        {
            DbContextOptions<Models.ECommerceContext> options = new DbContextOptions<Models.ECommerceContext>();
            Models.ECommerceContext eCommerceContext = new Models.ECommerceContext(options);
            Areas.Seller.Controllers.ProductsController productsController = new Areas.Seller.Controllers.ProductsController(eCommerceContext);
            Models.Product product = productsController.Product(id);

            float subTotal = 0;
            ChangeCookie(id, count);
            subTotal = product.ProductPrice * count;
            return subTotal.ToString();

        }
        private void ChangeCookie(long id, byte count)
        {
            DbContextOptions<Models.ECommerceContext> options = new DbContextOptions<Models.ECommerceContext>();
            Models.ECommerceContext eCommerceContext = new Models.ECommerceContext(options);
            CustomersController customersController = new CustomersController(eCommerceContext);
           

            string cart = Request.Cookies["cart"];
            string[] cartItems;
            string[] itemDetails;
            short itemCount;
            string newCart = "";
            string cartItem;
            short totalCount = 0;

            CookieOptions cookieOptions = new CookieOptions();

            cartItems = cart.Split(",");

            for (short i = 0; i < cartItems.Length; i++)
            {
                cartItem = cartItems[i];
                itemDetails = cartItem.Split(":");
                itemCount = Convert.ToInt16(itemDetails[1]);

                if (itemDetails[0] == id.ToString())
                {

                    itemCount = count;
                }

                totalCount += itemCount;
                newCart = newCart + itemDetails[0] + ":" + itemCount.ToString();
                if (i < cartItems.Length - 1)
                {
                    newCart = newCart + ",";
                }

            }
            if (newCart != "")
            {
                if (newCart.Substring(cart.Length - 1) == ",")
                {
                    newCart = newCart.Substring(0, newCart.Length - 1);
                    //newCart.Remove(newCart.Length - 1);
                }
            }
            else
            {
                Response.Cookies.Delete("cart");
                return;
            }
            
            cookieOptions.Path = "/";
            cookieOptions.Expires = DateTime.MaxValue;
            Response.Cookies.Append("cart", newCart, cookieOptions);
            if (this.HttpContext.Session.GetString("customer") != null)
            {
                customersController.TransferCard(Convert.ToInt64(this.HttpContext.Session.GetString("customer")),this.HttpContext,eCommerceContext,newCart);
            }

        }
        public void EmptyBasket()
        {

            DbContextOptions<Models.ECommerceContext> options = new DbContextOptions<Models.ECommerceContext>();
            Models.ECommerceContext eCommerceContext = new Models.ECommerceContext(options);
            CustomersController customersController = new CustomersController(eCommerceContext);

            Response.Cookies.Delete("cart");
            if (this.HttpContext.Session.GetString("customer") != null)
            {
                customersController.TransferCard(Convert.ToInt64(this.HttpContext.Session.GetString("customer")), this.HttpContext, eCommerceContext, "");
            }
            Response.Redirect("Index");
        }
    }
}

