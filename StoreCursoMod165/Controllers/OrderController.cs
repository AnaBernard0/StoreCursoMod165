using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreCursoMod165.Data;
using StoreCursoMod165.Models;

namespace StoreCursoMod165.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Order> orders = _context.Orders
                                                    .Include(o=>o.Customer)
                                                    .Include(o=>o.Product)
                                                    .Include(o=>o.Status)
                                                    .ToList();

            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            this.SetUpOrderModel();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                //Criar new category
                _context.Orders.Add(order);
               // Console.WriteLine(order.CategoryID);
                //Console.WriteLine(product.Category.Name);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            this.SetUpOrderModel();
            return View(order);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Order? order = _context.Orders.Find(id);
            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Order? order = _context.Orders.Find(id);
            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }
            this.SetUpOrderModel();
            return View(order);
        }
        [HttpPost]
        public IActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Orders.Update(order);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Order? order = _context.Orders.Find(id);
            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }
            this.SetUpOrderModel();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Order? order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        private void SetUpOrderModel()
        {
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");
            ViewBag.ProductList = new SelectList(_context.Products, "ID", "Description");
            ViewBag.Status = new SelectList(_context.OrderStatus, "ID", "NameofStatus");

        }
    }
}
