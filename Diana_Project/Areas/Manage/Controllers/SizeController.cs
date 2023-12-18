using Microsoft.AspNetCore.Mvc;

namespace Diana_Project.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SizeController : Controller
    {
        AppDbContext _context;
        public SizeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Size> sizes = _context.Sizes.ToList();
            return View(sizes);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Size size)
        {
            _context.Sizes.Add(size);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            Size size = _context.Sizes.FirstOrDefault(c => c.Id == id);
            return View(size);
        }
        [HttpPost]
        public IActionResult Update(Size newSize)
        {
            Size oldSize = _context.Sizes.FirstOrDefault(c => c.Id == newSize.Id);
            oldSize.Name = newSize.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Size size = _context.Sizes.FirstOrDefault(c => c.Id == id);
            _context.Sizes.Remove(size);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
