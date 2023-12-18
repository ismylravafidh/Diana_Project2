using System.ComponentModel.DataAnnotations.Schema;

namespace Diana_Project.Areas.Manage.ViewModels.Slider
{
    public class UpdateSliderVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string? ImgUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
