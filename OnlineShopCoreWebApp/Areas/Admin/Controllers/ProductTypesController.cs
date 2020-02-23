using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopCoreWebApp.Data;
using OnlineShopCoreWebApp.Models;

namespace OnlineShopCoreWebApp.Areas.Customer.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        public ProductTypesController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public IActionResult Index()
        {
            return View(_applicationDbContext.ProductTypes.ToList());
        }
        [HttpPost]
        public IActionResult Index(string type)
        {
            var products = _applicationDbContext.ProductTypes.Where(x => x.ProductType.StartsWith(type)).ToList();
            if (type == null )
            {
                products = _applicationDbContext.ProductTypes.ToList();

            }
            return View(products);
        }
        //For Create Get Action Method
        public ActionResult Create()
        {
            return View();
        }
        //Create post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _applicationDbContext.ProductTypes.Add(productTypes);
                await _applicationDbContext.SaveChangesAsync();
                TempData["save"] = "Product saved successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        //For Edit Get Action Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var productType = _applicationDbContext.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
           
            return View(productType);
        }
        //Edit post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _applicationDbContext.Update(productTypes);
                await _applicationDbContext.SaveChangesAsync();
                TempData["save"] = "Product updated successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }
        //For Details Get Action Method
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var productType = _applicationDbContext.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }
        //Details post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ProductTypes productTypes)
        {
                return RedirectToAction(nameof(Index));
          
        }
        //For Delete Get Action Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var productType = _applicationDbContext.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }
        //Delete post action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id,ProductTypes productTypes)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (id != productTypes.Id)
            {
                return NotFound();

            }
            var productType = _applicationDbContext.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();

            }

            if (ModelState.IsValid)
            {
                _applicationDbContext.Remove(productType);
                await _applicationDbContext.SaveChangesAsync();
                TempData["save"] = "Product deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }
    }
}