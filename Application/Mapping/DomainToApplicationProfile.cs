using AutoMapper;
using ApiEcommerce.Domain.Entities;
using ApiEcommerce.Application.DTOs;

namespace ApiEcommerce.Application.Mapping
{
    /// <summary>
    /// 🗂️ AutoMapper Profile para Clean Architecture
    /// ✅ Solo mappings básicos que funcionan
    /// </summary>
    public class DomainToApplicationProfile : Profile
    {
        public DomainToApplicationProfile()
        {
            // 🏷️ CATEGORY MAPPINGS BÁSICOS
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate));

            // 🛍️ PRODUCT MAPPINGS BÁSICOS
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate));

            // 👤 USER MAPPINGS BÁSICOS
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
        }
    }
}