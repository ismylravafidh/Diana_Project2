using Diana_Project.Areas.Manage.ViewModels.Product;
using Diana_Project.Models;

namespace Diana_Project.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        AppDbContext _context;
        IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Product> products =_context.Products.Where(p=>p.IsDeleted==false)
                .Include(p=>p.Images)
                .Include(p=>p.ProductColors).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductMaterials).ThenInclude(p=>p.Material)
                .Include(p=>p.ProductSizes).ThenInclude(p=>p.Size).ToList();
            return View(products);
        }
        public IActionResult Create()
        {
			ViewBag.Colors = _context.Colors.ToList();
			ViewBag.Materials = _context.Materials.ToList();
			ViewBag.Sizes = _context.Sizes.ToList();
			return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductVm productVm)
        
        {
			ViewBag.Colors = _context.Colors.ToList();
			ViewBag.Materials = _context.Materials.ToList();
			ViewBag.Sizes = _context.Sizes.ToList();

			if (!ModelState.IsValid)
            {
                return View();
            }
            Product product = new Product()
            {
                Name = productVm.Name,
                Description = productVm.Description,
                Price = productVm.Price,
                IsDeleted=false,
                Images = new List<Image>()
            };
            if (productVm.ColorIds != null)
            {
                foreach(var colorId in productVm.ColorIds)
                {
                    ProductColor color = new ProductColor()
                    {
                        Product=product,
                        ColorId=colorId,
                    };
                    _context.ProductColors.Add(color);
                }
            }
            if (productVm.MaterialIds != null)
            {
                foreach (var materialId in productVm.MaterialIds)
                {
                    ProductMaterial material = new ProductMaterial()
                    {
                        Product = product,
                        MaterialId = materialId,
                    };
                    _context.ProductMaterials.Add(material);
                }
            }
            if (productVm.SizeIds != null)
            {
                foreach (var sizeId in productVm.SizeIds)
                {
                    ProductSize size = new ProductSize()
                    {
                        Product = product,
                        SizeId = sizeId,
                    };
                    _context.ProductSizes.Add(size);
                }
            }

            if (!productVm.MainImage.ContentType.Contains("image"))
            {
                ModelState.AddModelError("MainImage", "Yalnizca Sekil yukluye bilersiz");
                return View();
            }
            Image MainImage = new Image()
            {
                IsImage=true,
                ImgUrl= productVm.MainImage.Upload(_env.WebRootPath, @"\Upload\Product\"),
                Product=product,
            };
            product.Images.Add(MainImage);

            if(productVm.Images != null)
            {
                foreach(var images in productVm.Images)
                {
                    if (!images.ContentType.Contains("image"))
                    {
                        ModelState.AddModelError("Images", "Yalnizca Sekil yukluye bilersiz");
                        continue;
                    }
                    Image newImages = new Image()
                    {
                        IsImage = false,
                        ImgUrl=images.Upload(_env.WebRootPath, @"\Upload\Product\"),
                        Product = product
                    };
                    product.Images.Add(newImages);
                }
            }

            _context.Products.Add(product);
            
            _context.SaveChanges();
            
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            Product product = _context.Products.Where(p => p.IsDeleted == false)
                .Include(p => p.Images)
                .Include(p => p.ProductColors).ThenInclude(p => p.Color)
                .Include(p => p.ProductMaterials).ThenInclude(p => p.Material)
                .Include(p => p.ProductSizes).ThenInclude(p => p.Size)
                .Where(p => p.Id == id).FirstOrDefault();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Materials = _context.Materials.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();

            UpdateProductVm updateProductVm = new UpdateProductVm()
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ColorIds = new List<int>(),
                MaterialIds = new List<int>(),
                SizeIds = new List<int>(),
                productImages = new List<ProductImagesVm>()
            };

            foreach(var item in product.ProductColors)
            {
                updateProductVm.ColorIds.Add(item.ColorId);
            }
            foreach (var item in product.ProductMaterials)
            {
                updateProductVm.MaterialIds.Add(item.MaterialId);
            }
            foreach (var item in product.ProductSizes)
            {
                updateProductVm.SizeIds.Add(item.SizeId);
            }

            foreach (var item in product.Images)
            {
                ProductImagesVm productImages = new ProductImagesVm()
                {
                    Id = item.Id,
                    IsImage = item.IsImage,
                    ImgUrl = item.ImgUrl
                };
                updateProductVm.productImages.Add(productImages);
            }


            return View(updateProductVm);
        }
        [HttpPost]
        public IActionResult Update(UpdateProductVm newProduct)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Materials = _context.Materials.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();

            Product oldProduct = _context.Products.Where(p => p.IsDeleted == false)
                .Include(p => p.Images)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductMaterials)
                .Include(p => p.ProductSizes)
                .Where(p => p.Id == newProduct.Id).FirstOrDefault();

            oldProduct.Name = newProduct.Name;
            oldProduct.Description = newProduct.Description;
            oldProduct.Price = newProduct.Price;

            if (newProduct.ColorIds != null)
            {
                List<int> createColors;
                if (oldProduct.ProductColors != null)
                {
                    createColors = newProduct.ColorIds.Where(ti => !oldProduct.ProductColors.Exists(pt => pt.ColorId == ti)).ToList();
                }
                else
                {
                    createColors = newProduct.ColorIds.ToList();
                }
                foreach (int colorId in createColors)
                {
                    ProductColor productColor = new ProductColor()
                    {
                        ColorId = colorId,
                        ProductId = oldProduct.Id
                    };
                    _context.ProductColors.Add(productColor);

                }
                List<ProductColor> removeColors = oldProduct.ProductColors.Where(p => !newProduct.ColorIds.Contains(p.ColorId)).ToList();

                _context.ProductColors.RemoveRange(removeColors);
            }
            else
            {
                var productColorList = _context.ProductColors.Where(pt => pt.ProductId == oldProduct.Id).ToList();
                _context.ProductColors.RemoveRange(productColorList);
            }
            if (newProduct.MaterialIds != null)
            {
                List<int> createMaterials;
                if (oldProduct.ProductColors != null)
                {
                    createMaterials = newProduct.MaterialIds.Where(ti => !oldProduct.ProductMaterials.Exists(pt => pt.MaterialId == ti)).ToList();
                }
                else
                {
                    createMaterials = newProduct.MaterialIds.ToList();
                }
                foreach (int materialId in createMaterials)
                {
                    ProductMaterial productMaterial = new ProductMaterial()
                    {
                        MaterialId = materialId,
                        ProductId = oldProduct.Id
                    };
                    _context.ProductMaterials.Add(productMaterial);

                }
                List<ProductMaterial> removeMaterials = oldProduct.ProductMaterials.Where(p => !newProduct.MaterialIds.Contains(p.MaterialId)).ToList();

                _context.ProductMaterials.RemoveRange(removeMaterials);
            }
            else
            {
                var productMaterialList = _context.ProductMaterials.Where(pt => pt.ProductId == oldProduct.Id).ToList();
                _context.ProductMaterials.RemoveRange(productMaterialList);
            }
            if (newProduct.SizeIds != null)
            {
                List<int> createSizes;
                if (oldProduct.ProductSizes != null)
                {
                    createSizes = newProduct.SizeIds.Where(ti => !oldProduct.ProductSizes.Exists(pt => pt.SizeId == ti)).ToList();
                }
                else
                {
                    createSizes = newProduct.SizeIds.ToList();
                }
                foreach (int sizeId in createSizes)
                {
                    ProductSize productSize = new ProductSize()
                    {
                        SizeId = sizeId,
                        ProductId = oldProduct.Id
                    };
                    _context.ProductSizes.Add(productSize);

                }
                List<ProductSize> removeSizes = oldProduct.ProductSizes.Where(p => !newProduct.SizeIds.Contains(p.SizeId)).ToList();

                _context.ProductSizes.RemoveRange(removeSizes);
            }
            else
            {
                var productSizeList = _context.ProductSizes.Where(pt => pt.ProductId == oldProduct.Id).ToList();
                _context.ProductSizes.RemoveRange(productSizeList);
            }
            if (newProduct.MainImage != null)
            {
                if (!newProduct.MainImage.CheckType("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "Duzgun formatda sekil daxil edin!");
                    return View();
                }
                Image newMainImages = new Image()
                {
                    IsImage = true,
                    ProductId = oldProduct.Id,
                    ImgUrl = newProduct.MainImage.Upload(_env.WebRootPath, @"\Upload\Product\")
                };
                var oldmainImage = oldProduct.Images?.FirstOrDefault(p => p.IsImage == true);
                oldProduct.Images?.Remove(oldmainImage);
                oldProduct.Images.Add(newMainImages);
            }
            if (newProduct.Images != null)
            {
                foreach (var item in newProduct.Images)
                {
                    if (!item.CheckType("image/"))
                    {
                        TempData["Error"] += $"{item.FileName} type duzgun deyil  \t";
                        continue;
                    }
                    Image newImage = new Image()
                    {
                        IsImage = false,
                        ImgUrl = item.Upload(_env.WebRootPath, @"\Upload\Product\"),
                        Product = oldProduct,
                    };
                    oldProduct.Images.Add(newImage);
                }
            }

            if (newProduct.ImageIds != null)
            {
                var removeListImage = oldProduct.Images?.Where(p => !newProduct.ImageIds.Contains(p.Id) && p.IsImage == false).ToList();
                foreach (var item in removeListImage)
                {
                    oldProduct.Images.Remove(item);
                    item.ImgUrl.DeleteFile(_env.WebRootPath, @"\Upload\Product\");
                }
            }
            else
            {
                oldProduct.Images.RemoveAll(p => p.IsImage == false);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Where(p=>p.IsDeleted==false).FirstOrDefault(p=>p.Id==id);

            product.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
