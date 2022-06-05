using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<E_Commerce.Models.ECommerceContext>();  //source 127.0.0.1 yazýlabilir
builder.Services.AddDbContext<E_Commerce.Areas.Admin.Models.UserContext>(x => x.UseSqlServer("Data Source=KK3408;Initial Catalog=ECommerce; Integrated Security=True"));  //source 127.0.0.1 yazýlabilir
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<E_Commerce.Models.ECommerceContext>(x => x.UseSqlServer("Data Source=KK3408;Initial Catalog=ECommerce; Integrated Security=True"));  //source 127.0.0.1 yazýlabilir

builder.Services.AddSession();

CultureInfo cultureInfo = new CultureInfo("tr-TR");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
var app = builder.Build();
app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
   name: "areas",
   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
