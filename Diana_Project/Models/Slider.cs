using System.ComponentModel.DataAnnotations.Schema;

namespace Diana_Project.Models
{
    public class Slider : BaseEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string? ImgUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
