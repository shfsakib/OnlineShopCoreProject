using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopCoreWebApp.Data;
using OnlineShopCoreWebApp.Models;
using OnlineShopCoreWebApp.Utility;
using X.PagedList;

namespace OnlineShopCoreWebApp.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index(int? page)
        {
            return View(_applicationDbContext.Products.Include(x=>x.ProductTypes).Include(x=>x.SpecialTags).ToList().ToPagedList(page??1,9));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //Get product details action method
        public ActionResult Detail(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var product = _applicationDbContext.Products.Include(x=>x.ProductTypes).FirstOrDefault(x=>x.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //Post product details action method
        [HttpPost]
        [ActionName("Detail")]
        public ActionResult ProductDetail(int? id)
        {
            List<Products> products = new List<Products>();
            if (id == null)
            {
                return NotFound();
            }
            var product = _applicationDbContext.Products.Include(x => x.ProductTypes).FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            products = HttpContext.Session.Get<List<Products>>("products");
            if (products==null)
            {
                products = new List<Products>();
            }
            products.Add(product);
            HttpContext.Session.Set("products",products);
            //return View(product);
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Products> products = new List<Products>();
            products=HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
              var product = products.FirstOrDefault(x => x.Id == id);
                if (product!=null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);

                }


            }

            return RedirectToAction(nameof(Cart));


        }

        //Get product cart action method 
        [HttpGet]
        public IActionResult Cart()
        {

            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products==null)
            {
                products = new List<Products>();
            }

            return View(products);
        }
    }
}
