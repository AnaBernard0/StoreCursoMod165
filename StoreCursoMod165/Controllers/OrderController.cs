using Microsoft.AspNetCore.Authorization;
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
            IEnumerable<Product> products = _context.Products
                                                    .Include(p => p.Category).ToList();

            IEnumerable<Order> orders = _context.Orders
                                                    .Include(o => o.Product)
                                                    .Include(o => o.Customer)
                                                    .Include(o=>o.Status)
                                                    .ToList();
            foreach(var order in orders)
            {
                order.TotalValue = Decimal.Parse(order.Quantity) * (order.Product.Price);
            }

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
                this.SetUpOrderModel();
                
                
                //Criar new category
                
                _context.Orders.Add(order);
                _context.SaveChanges();
               

                return RedirectToAction("Index");
            }
            this.SetUpOrderModel();
            //order.TotalValue = Decimal.Parse(order.Quantity) * (order.Product.Price);
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
            Order? order = _context.Orders
                                           .Include(o => o.Product)
                                           .Include(o => o.Customer)
                                           .Include(o => o.Status)
                                           .Where(o => o.ID == id)
                                           .Single();


            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }
            this.SetUpOrderModel();            
            return View(order);
        }

        [HttpGet]
        public IActionResult SeeOrders(Customer customer)
        {
            if (customer == null)
                return NotFound();
            var seeOrdersbyCustomer = _context.Orders
                                               .Include(o=>o.Product)
                                               .Include(o => o.Status)
                                               .Include(o=>o.Customer)
                                               .Where(o=>o.CustomerID == customer.ID)
                                               .ToList();
            this.SetUpOrderModel();
            var name = _context.Orders.Include(o => o.Customer).Where(o => o.CustomerID == customer.ID).FirstOrDefault();
            ViewBag.Name = name.Customer.Name;

            decimal totalPago = 0;
            decimal totalPorPagar = 0;
            foreach (var order in seeOrdersbyCustomer)
            {
                if(order.IsPaid)
                    totalPago=totalPago+order.TotalValue;
                totalPorPagar=totalPorPagar+order.TotalValue;
            }

            ViewBag.TotalPaid=totalPago;
            ViewBag.TotalPor=totalPorPagar;
            ViewBag.Soma=totalPago-totalPorPagar;
            return View(seeOrdersbyCustomer);
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

        [HttpGet]
        public IActionResult Ordered()
        {
            var seeOrdered = _context.Orders
                                               .Include(o => o.Product)
                                               .Include(o => o.Status)
                                               .Include(o => o.Customer)
                                               .Where(o=>o.StatusID == 1)
                                               .ToList();
            this.SetUpOrderModel();
          
            return View(seeOrdered);
        }

        private void SetUpOrderModel()
        {
            ViewBag.ProductList = new SelectList(_context.Products, "ID", "Description");
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");
            ViewBag.Status = new SelectList(_context.OrderStatus, "ID", "NameofStatus");

        }
    }
}
