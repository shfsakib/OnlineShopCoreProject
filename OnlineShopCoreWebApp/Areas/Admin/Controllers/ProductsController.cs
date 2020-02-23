using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopCoreWebApp.Data;
using OnlineShopCoreWebApp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineShopCoreWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags);
            return View(await applicationDbContext.ToListAsync());
        }
        //Post index action method
        [HttpPost]
        public IActionResult Index(decimal? lowAmount, decimal? largeAmount)
        {
            var products = _context.Products.Include(x => x.ProductTypes).Include(x => x.SpecialTags).Where(x => x.Price >= lowAmount && x.Price <= largeAmount).ToList();
            if (lowAmount == null || largeAmount == null)
            {
                products = _context.Products.Include(x => x.ProductTypes).Include(x => x.SpecialTags).ToList();
            }
            return View(products);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.SpecialTags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "ProductType");
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "TagName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Products products, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var searchProduct = _context.Products.FirstOrDefault(x => x.Name == products.Name);
                if (searchProduct != null)
                {
                    ViewBag.message = "This product already exist";
                    ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes.ToList(), "Id", "ProductType");
                    ViewData["TagId"] = new SelectList(_context.SpecialTags.ToList(), "Id", "Name");
                    return View(products);
                }
                if (image != null)
                {
                    var name = Path.Combine(_hostingEnvironment.WebRootPath + "/Images/", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.Image = "/Images/" + image.FileName;
                }

                if (image == null)
                {
                    products.Image = "/images/no_image.png";

                }
                _context.Add(products);
                await _context.SaveChangesAsync();
                TempData["save"] = "Product deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "ProductType", products.ProductTypeId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "TagName", products.SpecialTagId);
            return View(products);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "ProductType", products.ProductTypeId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "TagName", products.SpecialTagId);
            return View(products);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Image,ProductColor,IsAvailable,ProductTypeId,SpecialTagId")] Products products,IFormFile image)
        {
            if (id != products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var name = Path.Combine(_hostingEnvironment.WebRootPath + "/Images/", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.Image = "/Images/" + image.FileName;
                }

                if (image == null)
                {
                    products.Image = products.Image;

                }
                //var searchProduct = _context.Products.FirstOrDefault(x => x.Name == products.Name);
                //if (searchProduct != null)
                //{
                //    ViewBag.message = "This product already exist";
                //    ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes.ToList(), "Id", "ProductType");
                //    ViewData["TagId"] = new SelectList(_context.SpecialTags.ToList(), "Id", "Name");
                //    return View(products);
                //}
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["save"] = "Product updated successfully";

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "ProductType", products.ProductTypeId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "TagName", products.SpecialTagId);
            return View(products);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.SpecialTags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            TempData["save"] = "Product deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
