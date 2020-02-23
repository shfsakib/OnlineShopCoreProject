using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopCoreWebApp.Data;
using OnlineShopCoreWebApp.Models;
using OnlineShopCoreWebApp.Utility;

namespace OnlineShopCoreWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private object anOrder;

        public OrderController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        //Get checkout action method
        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }
        //post checkout action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order anOrder)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products!=null)
            {
                foreach (var Product in products)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.ProductId = Product.Id;

                    anOrder.orderDetails.Add(orderDetails);

                }
            }
            anOrder.OrderNo = GetOrderNo();
            _applicationDbContext.Orders.Add(anOrder);
            await _applicationDbContext.SaveChangesAsync();
            HttpContext.Session.Set("products",new List<Products>());
            return View();
        }


        public string GetOrderNo()
        {
            int rowCount = _applicationDbContext.Orders.ToList().Count() + 1;
            return rowCount.ToString("0000");
        }
    }
}