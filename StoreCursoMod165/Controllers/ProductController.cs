using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreCursoMod165.Data;
using StoreCursoMod165.Models;

namespace StoreCursoMod165.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _context.Products
                                                    .Include(p=>p.Category).ToList();

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            this.SetUpCategoryModel();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                //Criar new category
                _context.Products.Add(product);
                Console.WriteLine(product.CategoryID);
                //Console.WriteLine(product.Category.Name);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            this.SetUpCategoryModel();
            return View(product);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Product? product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product? product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            this.SetUpCategoryModel();
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product? product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            this.SetUpCategoryModel();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product? product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        private void SetUpCategoryModel()
        {
            ViewBag.Category = new SelectList(_context.Category, "ID", "Name");
            
        }
    }
}
