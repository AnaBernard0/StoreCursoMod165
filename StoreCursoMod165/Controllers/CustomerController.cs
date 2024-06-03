using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreCursoMod165.Data;
using StoreCursoMod165.Models;

namespace StoreCursoMod165.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Customer> customers = _context.Customers
                                                    .ToList();

            return View(customers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                //Criar new customer
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(customer);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Customer? customer = _context.Customers.Find(id);
            if (customer == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Customer? customer = _context.Customers.Find(id);
            if (customer == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Customer? customer = _context.Customers.Find(id);
            if (customer == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Customer? customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
    }
}
