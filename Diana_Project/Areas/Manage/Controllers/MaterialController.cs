using Microsoft.AspNetCore.Mvc;

namespace Diana_Project.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class MaterialController : Controller
    {
        AppDbContext _context;
        public MaterialController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Material> materials = _context.Materials.ToList();
            return View(materials);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Material material)
        {
            _context.Materials.Add(material);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            Material material = _context.Materials.FirstOrDefault(c => c.Id == id);
            return View(material);
        }
        [HttpPost]
        public IActionResult Update(Material newMaterial)
        {
            Material oldMaterial = _context.Materials.FirstOrDefault(c => c.Id == newMaterial.Id);
            oldMaterial.Name = newMaterial.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Material material = _context.Materials.FirstOrDefault(c => c.Id == id);
            _context.Materials.Remove(material);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
