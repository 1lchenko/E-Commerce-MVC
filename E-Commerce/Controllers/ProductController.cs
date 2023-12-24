using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Models.Home;
using ECommerce.Models.Home.ProductDTO;
using ECommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ECommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductDetailRepository _detailsRepository;
        
        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IProductDetailRepository detailsRepository)
        {
            _productRepository = productRepository;
            _detailsRepository = detailsRepository;
            _categoryRepository = categoryRepository;
        }
        
        
        
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(string? searchText, int page = 1)
        {
           ProductsPageViewModel productsPageViewModel;
           
            if (searchText != null)
            {
                productsPageViewModel = await _productRepository.GetProductsByIndexOfAsync(searchText, page);
                ViewBag.searchText = searchText;
            }
            else
            {
                productsPageViewModel = await _productRepository.GetAllProductAsync(page);
                ViewBag.searchText = null;
            }
             
            return View(productsPageViewModel);
        }

        
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Categories = await _categoryRepository.GetAllCategoriesAsync();
            
            return View("ProductAddEdit/AddProduct");
        }

        
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductAddDTO newProduct)
        {
            
            if (newProduct.ImageFiles?.Count > 3)
            {
                ModelState.AddModelError("maxImageError", "You can upload only 3 image");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    await _productRepository.CreateProductAsync(newProduct);
                    return RedirectToAction(nameof(GetAllProducts));

                }
            }
            catch (AddImageException ex)
            {
                ModelState.AddModelError("imageError", ex.Message);
            }
            
            ViewBag.Categories = await _categoryRepository.GetAllCategoriesAsync();
            return View("ProductAddEdit/AddProduct", newProduct);
            
        }

       
        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        { 

            ProductEditDTO? productEditDto = await _productRepository.GetProductEditDtoByIdAsync(productId);

            if (productEditDto == null)
            {
                return NotFound("Product not found for edit operation");
            }
            
            ViewBag.Categories = await _categoryRepository.GetAllCategoriesAsync();
             
            return View("ProductAddEdit/EditProduct", productEditDto);
        }

       
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductEditDTO editProduct)
        { 

            if (ModelState.IsValid)
            {
                var result = await _productRepository.EditProductAsync(editProduct);

                if (!result)
                {
                    return NotFound("Product not found for edit operation");
                }
                
                return RedirectToAction("GetAllProducts");
            }

            ViewBag.Categories = await _categoryRepository.GetAllCategoriesAsync();
            return View("ProductAddEdit/EditProduct", editProduct);
        }

        
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId, string? searchText)
        {
           bool result = await _productRepository.DeleteProductAsync(productId);

           if (!result)
           {
               return NotFound("Product not found for deleting operation");
           }
           
           return RedirectToAction(nameof(GetAllProducts) ,new {searchText = searchText});
        }

        
        public async Task<IActionResult> AddDetailsDescription(int productId)
        {
           var product = await _productRepository.GetProductAsync(productId);

           if (product == null) 
           {
               return NotFound("Product is not found for add Details Description to it"); 
           }
           
           ViewBag.Product = product;
           return View("AddEditDetailsDescription/AddDetailsDescription", new List<ProductDetail>());

        }
        
        [HttpPost]
        public async Task<IActionResult> AddDetailsDescription(List<ProductDetail> productDetails)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _detailsRepository.AddProductsDetailsAsync(productDetails);
                    return RedirectToAction(nameof(GetAllProducts));
                }
                catch (AddProductsDetailsException ex)
                {
                    return BadRequest(ex.Message);
                }
                 
            }

            var product = await _productRepository.GetProductAsync(productDetails[0].ProductId);
            
            if (product == null)
            {
                return NotFound("Product not found for Add Details Description");
            }
            
            ViewBag.Product = product;
            return View("AddEditDetailsDescription/AddDetailsDescription", productDetails);

        }

        [HttpGet]
        public async Task<IActionResult> EditProductDetail(int productId )
        {
             
            var listProductDetails = await _detailsRepository.GetProductsDetailsAsync(productId);
            
            return View("AddEditDetailsDescription/EditProductDetail", listProductDetails);
             
        }

        [HttpPost]
        public async Task<IActionResult> EditProductDetail(ProductDetail productDetail)
        {
            
            if (ModelState.IsValid)
            {
                var result = await _detailsRepository.EditProductsDetailAsync(productDetail);
               
                if (!result)
                {
                    return NotFound("Product Detail isn`t exist for update operation");
                }
            }
            
            List<ProductDetail>  listProductDetails = await _detailsRepository.GetProductsDetailsAsync(productDetail.ProductId);
            return View("AddEditDetailsDescription/EditProductDetail", listProductDetails);
            
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductDetail(int productDetailId)
        {

            var result = await _detailsRepository.DeleteProductsDetailAsync(productDetailId);

            if (!result)
            {
                return NotFound("This Product Detail not exist for deleting");
            }
            
            return Ok();
        }


    }
}