using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using NToastNotify;
using StoreCursoMod165.Data;
using StoreCursoMod165.Models;

namespace StoreCursoMod165.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlLocalizer<Resource> _sharedLocalizer;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IToastNotification _toastNotification;

        public CategoryController(ApplicationDbContext context,
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
            IEnumerable<Category> categories = _context.Category.ToList();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //Criar new category
                _context.Category.Add(category);
                _context.SaveChanges();
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Category successfully created!"].Value);

                return RedirectToAction("Index");
            }
            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the category again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Category Creation error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });

            return View(category);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Category? category = _context.Category.Find(id);
            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category? category = _context.Category.Find(id);
            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Category.Update(category);
                _context.SaveChanges();
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Category successfully edited!"].Value);
                return RedirectToAction(nameof(Index));
            }
            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the category again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Category Edition error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category? category = _context.Category.Find(id);
            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category? category = _context.Category.Find(id);
            if (category != null)
            {
                _context.Category.Remove(category);
                _context.SaveChanges();
                //Notification success
                _toastNotification.AddSuccessToastMessage(_sharedLocalizer["Category successfully deleted!"].Value);
                return RedirectToAction(nameof(Index));
            }
            //notification error
            _toastNotification.AddErrorToastMessage(_sharedLocalizer["Check the category again!"].Value,
               new ToastrOptions
               {
                   Title = _sharedLocalizer["Category Deletion error!"].Value,
                   TapToDismiss = true,
                   TimeOut = 0
               });
            return View(category);
        }
    }
}
