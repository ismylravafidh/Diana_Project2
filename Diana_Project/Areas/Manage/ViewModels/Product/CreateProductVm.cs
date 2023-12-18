namespace Diana_Project.Areas.Manage.ViewModels.Product
{
    public class CreateProductVm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public List<int>? ColorIds { get; set; }
        public List<int>? SizeIds { get; set; }
        public List<int>? MaterialIds { get; set; }
        public IFormFile MainImage { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
