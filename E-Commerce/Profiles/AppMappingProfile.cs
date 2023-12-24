using AutoMapper;
using ECommerce.Models;
using ECommerce.Models.Home.ProductDTO;

namespace ECommerce.Profiles
{

    public class AppMappingProfile : Profile
        {
            public AppMappingProfile()
            {
                CreateMap<Product, ProductShowDTO>()
                    .ForMember(dest => dest.CategoryName, 
                        opt =>
                    {
                        opt.MapFrom(src => src.Category.CategoryName);
                    })
                    .ReverseMap();
                
                CreateMap<ProductAddDTO, Product>().ReverseMap();
                
                CreateMap<ProductEditDTO, Product>().ReverseMap();

                CreateMap<Category, Category>().ReverseMap();
                
                CreateMap<ProductDetail, ProductDetail>().ReverseMap();
                
                CreateMap<OrderAddDTO, Order>().ReverseMap();
                
                CreateMap<Order, Order>().ReverseMap();
                
               







            }
        }
     
}
