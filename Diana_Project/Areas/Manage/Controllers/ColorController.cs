using Microsoft.AspNetCore.Mvc;

namespace Diana_Project.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ColorController : Controller
    {
        AppDbContext _context;
        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Color> colors = _context.Colors.ToList();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Color color)
        {
            _context.Colors.Add(color);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            Color color = _context.Colors.FirstOrDefault(c => c.Id == id);
            return View(color);
        }
        [HttpPost]
        public IActionResult Update(Color newColor)
        {
            Color oldColor = _context.Colors.FirstOrDefault(c=>c.Id==newColor.Id);
            oldColor.Name=newColor.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Color color= _context.Colors.FirstOrDefault(c=>c.Id==id);
            _context.Colors.Remove(color);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
