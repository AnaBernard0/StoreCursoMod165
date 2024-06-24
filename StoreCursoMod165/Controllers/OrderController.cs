﻿using Microsoft.AspNetCore.Authorization;
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
                var products = _context.Products.ToList();
                decimal price = 0;
                //Calcula o preco da venda de acordo com os produtos seliconados
                foreach(var xy in products)
                {
                    if(xy.ID==order.ProductID)
                        price=xy.Price;
                }
               
                //Criar new category
                order.TotalValue = Decimal.Parse(order.Quantity) * (price);
                _context.Orders.Add(order);
                
               // 
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
        public IActionResult SeeOrders(Customer customer)
        {
            if (customer == null)
                return NotFound();
            var seeOrdersbyCustomer = _context.Orders
                                               .Include(o => o.Product)
                                               .Include(o => o.Status)
                                               .Include(o => o.Customer)
                                               .Where(o => o.CustomerID == customer.ID)
                                               .ToList();
            this.SetUpOrderModel();
            var name = _context.Orders.Include(o => o.Customer).Where(o => o.CustomerID == customer.ID).FirstOrDefault();
            ViewBag.Name = name.Customer.Name;

            decimal totalPago = 0;
            decimal totalPorPagar = 0;
            foreach (var order in seeOrdersbyCustomer)
            {
                if (order.IsPaid)
                    totalPago = totalPago + order.TotalValue;
                totalPorPagar = totalPorPagar + order.TotalValue;
            }

            ViewBag.TotalPaid = totalPago;
            ViewBag.TotalPor = totalPorPagar;
            ViewBag.Soma = totalPago - totalPorPagar;
            return View(seeOrdersbyCustomer);
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

        [HttpGet]
        public IActionResult InProgress()
        {
            var inprogress = _context.Orders
                                               .Include(o => o.Product)
                                               .Include(o => o.Status)
                                               .Include(o => o.Customer)
                                               .Where(o => o.StatusID == 2)
                                               .ToList();

            
            this.SetUpOrderModel();

            return View(inprogress);
        }

        [HttpGet]
        public IActionResult EditInProgress(int id)
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
        [HttpPost]
        public IActionResult EditInProgress(Order order)
        {
             order = _context.Orders
                                           .Include(o => o.Product)
                                           .Include(o => o.Customer)
                                           .Include(o => o.Status)
                                           .Where(o => o.ID==order.ID)
                                           .Single();


            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else 
            {
                    //Verifica a existencia do produto, se nao existir abate o preco para 0 
                    if (order.StatusID == 2 && ((Convert.ToInt32(order.Product.Quantity)) - (Convert.ToInt32(order.Quantity)) < 0))
                    {
                        order.Informations = "There is no more " + order.Product.Description;
                        order.TotalValue = 0;

                    }    
                    else
                       order.StatusID = 3;
                
            }
            //Guarda a encomenda como processada
            _context.SaveChanges();
            this.SetUpOrderModel();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Processed()
        {
            var seeOrdered = _context.Orders
                                               .Include(o => o.Product)
                                               .Include(o => o.Status)
                                               .Include(o => o.Customer)
                                               .Where(o => o.StatusID == 3)
                                               .ToList();
            this.SetUpOrderModel();

            return View(seeOrdered);
        }

        [HttpGet]
        public IActionResult EditProcessed(int id)
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
        [HttpPost]
        public IActionResult EditProcessed(Order order)
        {

            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //Atualiza stocks dos produtos
                if (order.StatusID == 3)
                {
                    order = _context.Orders
                                                   .Include(o => o.Product)
                                                   .Include(o => o.Customer)
                                                   .Where(o => o.ID == order.ID)
                                                   .Single();
                    order.Product.Quantity = Convert.ToString(Convert.ToInt32(order.Product.Quantity) - Convert.ToInt32(order.Quantity));
                    order.StatusID = 4;
                }
                //ENVIA EMAIL AO CLIENTE FINAL
            }
            _context.SaveChanges();
            this.SetUpOrderModel();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Sent()
        {
            var inprogress = _context.Orders
                                               .Include(o => o.Product)
                                               .Include(o => o.Status)
                                               .Include(o => o.Customer)
                                               .Where(o => o.StatusID == 4)
                                               .ToList();


            this.SetUpOrderModel();

            return View(inprogress);
        }
        private void SetUpOrderModel()
        {
            ViewBag.ProductList = new SelectList(_context.Products, "ID", "Description");
            ViewBag.CustomerList = new SelectList(_context.Customers, "ID", "Name");
            ViewBag.Status = new SelectList(_context.OrderStatus, "ID", "NameofStatus");

        }
    }
}
