using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NToastNotify;
using StoreCursoMod165.Data;
using StoreCursoMod165.Models;
using System.Text;

namespace StoreCursoMod165.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IToastNotification _toastNotification;
        private readonly IEmailSender _emailSender;

        public OrderController(ApplicationDbContext context,
                                IHtmlLocalizer<Resource> sharedLocalizer,
                                    IStringLocalizer<Resource> localizer,
                                    IToastNotification toastNotification,
                                    IEmailSender emailSender)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _context.Products
                                                    .Include(p => p.Category).ToList();

            IEnumerable<Order> orders = _context.Orders
                                                    .Include(o => o.Product)
                                                    .Include(o => o.Customer)
                                                    .Include(o => o.Status)
                                                    .ToList();
            foreach (var order in orders)
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
                foreach (var xy in products)
                {
                    if (xy.ID == order.ProductID)
                        price = xy.Price;
                }

                //Criar new category
                order.TotalValue = Decimal.Parse(order.Quantity) * (price);
                order.StatusID = 1;
                _context.Orders.Add(order);


                _context.SaveChanges();
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Order successfully created!"].Value);

                return RedirectToAction("Index");
            }

            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the order again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Order Creation error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
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
            //Customer? customer = _context.Customers.Find(order.CustomerID);
            //Product? product = _context.Products.Find(order.ProductID);
            //Status? status = _context.OrderStatus.Find(order.StatusID);
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

                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Order successfully edited!"].Value);

                return RedirectToAction("Index");
            }

            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the order again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Order Edition error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
            this.SetUpOrderModel();
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
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Order successfully deleted!"].Value);

                return RedirectToAction(nameof(Index));
            }

            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the order again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Order Deletion error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
            this.SetUpOrderModel();
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
                                               .Where(o => o.StatusID == 1)
                                               .ToList();
            this.SetUpOrderModel();

            return View(seeOrdered);
        }

        [HttpGet]
        public IActionResult EditOrdered(int id)
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
        public IActionResult EditOrdered(Order order)
        {
            Customer? customer = _context.Customers.Find(order.CustomerID);
            Product? product = _context.Products.Find(order.ProductID);
            Status? status = _context.OrderStatus.Find(order.StatusID);


            if (order == null || !ModelState.IsValid)
            {
                //error edition 
                _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the order again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Order Edition error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //Muda estado da venda encomendado opara em processamento
                if (order.StatusID == 1)
                {
                    order.StatusID = 2;

                }
                else
                    order.StatusID = 2;

            }
            //Guarda a encomenda como processada

            _context.Orders.Update(order);
            _context.SaveChanges();

            //Notification success
            _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Order edited to In Progress!"].Value);



            this.SetUpOrderModel();
            return RedirectToAction(nameof(Index));
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
            Customer? customer = _context.Customers.Find(order.CustomerID);
            Product? product = _context.Products.Find(order.ProductID);
            Status? status = _context.OrderStatus.Find(order.StatusID);


            if (order == null || !ModelState.IsValid || product == null || status == null || customer == null)
            {
                //error edition 
                _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the order again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Order Edition error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //Verifica a existencia do produto, se nao existir abate o preco para 0 
                if (order.StatusID == 2 && ((Convert.ToInt32(product.Quantity)) - (Convert.ToInt32(order.Quantity)) < 0))
                {
                    order.Informations = "There is no more " + order.Product.Description;
                    _toastNotification.AddWarningToastMessage("There is no more " + order.Product.Description);
                    order.TotalValue = 0;

                }
                else
                    order.StatusID = 3;

            }
            //Guarda a encomenda como processada
            _context.Orders.Update(order);
            _context.SaveChanges();
            //Notification success
            _toastNotification.AddSuccessToastMessage("Order edited to Processed!");

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
            Customer? customer = _context.Customers.Find(order.CustomerID);
            Product? product = _context.Products.Find(order.ProductID);
            Status? status = _context.OrderStatus.Find(order.StatusID);

            if (order == null || !ModelState.IsValid || product == null || status == null || customer == null)
            {
                //error edition 
                _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the order again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Order Edition error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //Atualiza stocks dos produtos
                if (order.StatusID == 3)
                {
                    
                    order.Product.Quantity = Convert.ToString(Convert.ToInt32(order.Product.Quantity) - Convert.ToInt32(order.Quantity));
                    order.StatusID = 4;
                }
                //ENVIA EMAIL AO CLIENTE FINAL
               
                if (customer == null)
                {
                    return NotFound();
                }
                var culture = Thread.CurrentThread.CurrentCulture;

                string template = System.IO.File.ReadAllText(
                    Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "EmailTemplates",
                            $"sent_order.{culture.Name}.html"));

                StringBuilder htmlBody = new StringBuilder(template);
                htmlBody.Replace("##CUSTOMER_NAME##", customer.Name);
                htmlBody.Replace("##ORDER_DATE##", order.Date.ToShortDateString());
                htmlBody.Replace("##ORDER_TIME##", order.Time.ToShortTimeString());
                htmlBody.Replace("##ORDER_QUANTITY##", order.Quantity);
                htmlBody.Replace("##PRODUCT_DESCRIPTION##", product.Description);
                htmlBody.Replace("##TOTAL_VALUE##", order.TotalValue.ToString());



                _emailSender.SendEmailAsync(customer.Email, "Order Sent", htmlBody.ToString());
            }
            _context.Orders.Update(order);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Order Sent!"].Value);

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
