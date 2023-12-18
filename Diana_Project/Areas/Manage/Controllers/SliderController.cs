namespace Diana_Project.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        AppDbContext _context;
        IWebHostEnvironment _env;
        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Slider> sliderList = _context.Sliders.ToList();
            return View(sliderList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {

            if(!slider.ImageFile.ContentType.Contains("image"))
            {
                ModelState.AddModelError("ImageFile", "Yalnizca Sekil yukluye bilersiz");
                return View();
            }

            slider.ImgUrl = slider.ImageFile.Upload(_env.WebRootPath, @"\Upload\SliderImage\");

            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(p=>p.Id == id);

            UpdateSliderVm updateSliderVm = new UpdateSliderVm()
            {
                Id = slider.Id,
                Title=slider.Title,
                SubTitle=slider.SubTitle,
                ImgUrl=slider.ImgUrl,
            };
            return View(updateSliderVm);
        }
        [HttpPost]
        public IActionResult Update(UpdateSliderVm newSlider)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            Slider oldSlider = _context.Sliders.FirstOrDefault(p=>p.Id == newSlider.Id);
            oldSlider.Title = newSlider.Title;
            oldSlider.SubTitle = newSlider.SubTitle;
            if (newSlider.ImageFile != null)
            {
                if (!newSlider.ImageFile.CheckType("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "Duzgun formatda sekil daxil edin!");
                    return View();
                }
                newSlider.ImgUrl = newSlider.ImageFile.Upload(_env.WebRootPath, @"\Upload\SliderImage\");
                FileManager.DeleteFile(oldSlider.ImgUrl, _env.WebRootPath, @"\Upload\SliderImage\");
                //oldSlider.ImgUrl.DeleteFile(_env.WebRootPath, @"\Upload\SliderImage");
                oldSlider.ImgUrl = newSlider.ImgUrl;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            _context.Sliders.Remove(slider);
            FileManager.DeleteFile(slider.ImgUrl, _env.WebRootPath, @"\Upload\SliderImage\");

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
