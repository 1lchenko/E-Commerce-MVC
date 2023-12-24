using AutoMapper;
using ECommerce.Data;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Models.Home;
using ECommerce.Models.Home.ProductDTO;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace ECommerce.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EShopDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepository> _logger;
        private int PageSize { get; }
        
        public ProductRepository(EShopDbContext context, IMapper mapper,ILogger<ProductRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            PageSize = 15;
        }

        public async Task<Product?> GetProductByName(string name)
        {
            var product = await _context.Products
                 .Where(x => x.Name == name)
                 .FirstOrDefaultAsync();
            return product;

        }
        public async Task CreateProductAsync(ProductAddDTO productAddDto)
        {
            var product = _mapper.Map<Product>(productAddDto);
            
            if (productAddDto.ImageFiles != null && productAddDto.ImageFiles.Count > 0)
            {
                
                try
                {
                    product.Images =  await CreateImageListAsync(productAddDto.ImageFiles);
                }
                catch(Exception ex)
                {
                    
                    _logger.LogError("Method {NameMethod} throw exception. Time of throwing exception - {Date} .Exception data - {Data}", 
                            nameof(CreateProductAsync), DateTime.Now.ToJson() ,ex.ToJson());
                    throw new AddImageException("Failed add image to product",ex); 
                }
                
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            

        }

        public async Task<bool> DeleteProductAsync(int id)
        {
           var productToDelete =  await _context.Products
               .Include(x => x.Images)
               .FirstOrDefaultAsync(x => x.ProductId == id);

            if (productToDelete == null)
            {
                return false;
            }
            
            _context.Products.Remove(productToDelete); 
            await _context.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> EditProductAsync(ProductEditDTO productEditDto)
        {
            var product = await _context.Products
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.ProductId == productEditDto.ProductId);
            
            var existingImages = product?.Images;
            
            if (product == null)
            {
                return false;
            }
            
            _mapper.Map(productEditDto, product);
            
            if (productEditDto.ImageFiles != null)
            {
                var images = await CreateImageListAsync(productEditDto.ImageFiles);
                
                product.Images = images;
                
            }
            else
            {
                product.Images = productEditDto.DeleteAllImages ? null : existingImages;
            }
             
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;

        }

         

        public async Task<ProductsPageViewModel> GetAllProductAsync(int page)
        {
            IQueryable<Product> source = _context.Products;
            var count = source.Count();
             
            List<Product> products  = await _context.Products
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .Include(x => x.Category)
                    .Include(x => x.Images)
                    .ToListAsync();
           
           
            List<ProductShowDTO> productsShowDto = new List<ProductShowDTO>();
            
            foreach (var product in products)
            {
                var productShowDto = _mapper.Map<ProductShowDTO>(product);

                if (product.Images!.Count > 0)
                {
                    productShowDto.Images = GetListImageUrl(product.Images);
                }

                productsShowDto.Add(productShowDto);
            }
            
            var viewModel = new ProductsPageViewModel
            {
                Products = productsShowDto,
                PaginationInfo = new PageViewModel(count, page, PageSize)   
            };

            return viewModel;
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        }
        
        public async Task<ProductsPageViewModel> GetProductsByCategoryAsync(int? categoryId, int page)
        {
            IQueryable<Product> source = _context.Products.Where(x => x.CategoryId == categoryId);
            var count = source.Count();
            
            List<Product> products =  await _context.Products
                    .Where(x => x.CategoryId == categoryId)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .Include(i => i.Images)
                    .ToListAsync();
          
         
            List<ProductShowDTO> productsDto = new List<ProductShowDTO>();
            _mapper.Map(products, productsDto);

            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Images != null)
                {
                    productsDto[i].Images = GetListImageUrl(products[i].Images!);
                    
                }
            }

            var viewModel = new ProductsPageViewModel
            {
                Products = productsDto,
                PaginationInfo = new PageViewModel(count, page, PageSize)   
            };

            return viewModel;
        }

         
        public async Task<ProductsPageViewModel> GetProductsByIndexOfAsync(string searchText, int page)
        {
            var products = await _context.Products
                // ReSharper disable once StringIndexOfIsCultureSpecific.1
                .Where(x => x.Name.IndexOf(searchText) != -1)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .Include(i => i.Images)
                    .ToListAsync();
            
            var count = products.Count();
            
            List<ProductShowDTO> productsDto = new List<ProductShowDTO>();
            _mapper.Map(products, productsDto);

            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Images != null)
                {
                    productsDto[i].Images = GetListImageUrl(products[i].Images!);
                    
                }
            }

            var viewModel = new ProductsPageViewModel
            {
                Products = productsDto,
                PaginationInfo = new PageViewModel(count, page, PageSize)   
            };

            return viewModel;
        }
        
        public async Task<ProductShowDTO?> GetProductShowDtoByIdAsync(int id)
        {
            Product? product = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.Images)
                    .FirstOrDefaultAsync(x => x.ProductId == id);
            
            ProductShowDTO? productShowDto = _mapper.Map<ProductShowDTO>(product);

            if (product?.Images != null )
            {
                productShowDto.Images = GetListImageUrl(product.Images);
            }

            return productShowDto;
        }
        public async Task<ProductEditDTO?> GetProductEditDtoByIdAsync(int id)
        {
            Product? product = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            ProductEditDTO? productEditDto = _mapper.Map<ProductEditDTO>(product);
            
                if (product?.Images != null)
                {
                     
                    productEditDto.Images = GetListImageUrl(product.Images);
                }
                
            return productEditDto;
        }
        

        public async Task<List<Product>> FilteredProductsByIds(List<int> productIds)
        {
            var filteredProducts = await _context.Products
                                         .Where(p => productIds.Contains(p.ProductId))
                                         .ToListAsync();
            
            
            return filteredProducts;
        }
        
        private List<string> GetListImageUrl(List<Image> imagesListObj)
        {
            var listImages = new List<string>();
                    
            for (int i = 0; i < imagesListObj.Count; i++)
            {
                string imageBase64Data = Convert.ToBase64String(imagesListObj[i].ImageBytes);
                string imageDataUrl = $"data:image/png;base64,{imageBase64Data}";
                listImages.Add(imageDataUrl); 
            }

            return listImages;
        }
        private async Task<List<Image>> CreateImageListAsync(List<IFormFile> files)
        {
            List<Image> images = new List<Image>();
            foreach (var imageFile in files)
            {
                Image image = new Image();
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    image.ImageBytes = memoryStream.ToArray();
                }
                images.Add(image);
            }

            return images;
        }
        public class PageViewModel
        {
            public int PageNumber { get; private set; }
            public int TotalPages { get; private set; }

            public PageViewModel(int count, int pageNumber, int pageSize)
            {
                PageNumber = pageNumber;
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            }

            public bool HasPreviousPage
            {
                get
                {
                    return (PageNumber > 1);
                }
            }

            public bool HasNextPage
            {
                get
                {
                    return (PageNumber < TotalPages);
                }
            }



        }
    }
}