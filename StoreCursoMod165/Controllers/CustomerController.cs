using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NToastNotify;
using StoreCursoMod165.Data;
using StoreCursoMod165.Models;

namespace StoreCursoMod165.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IToastNotification _toastNotification;

        public CustomerController(ApplicationDbContext context,
                                IHtmlLocalizer<Resource> sharedLocalizer,
                                    IStringLocalizer<Resource> localizer,
                                    IToastNotification toastNotification)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
            _localizer = localizer;
            _toastNotification = toastNotification;
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
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Customer successfully created!"].Value);
                return RedirectToAction("Index");
            }
            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the customer again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Customer Creation error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
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
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Customer successfully edited!"].Value);
                return RedirectToAction(nameof(Index));
            }
            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the customer again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Customer Edition error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
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
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Customer successfully deleted!"].Value);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
    }
}
