namespace Diana_Project.Models
{
    public class Image : BaseEntity
    {
        public string ImgUrl { get; set; }
        public int ProductId { get; set; }
        public bool? IsImage { get; set; }
        public Product? Product { get; set; }
    }
}
