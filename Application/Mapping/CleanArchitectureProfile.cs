using AutoMapper;
using ApiEcommerce.Application.DTOs;
using DomainCategory = ApiEcommerce.Domain.Entities.Category;
using DomainProduct = ApiEcommerce.Domain.Entities.Product;
using DomainUser = ApiEcommerce.Domain.Entities.User;

namespace ApiEcommerce.Application.Mapping
{
    /// <summary>
    /// Perfiles de AutoMapper para Clean Architecture
    /// ✅ Mapea entre Domain Entities y DTOs
    /// ✅ Separado por responsabilidades
    /// ✅ Configuración centralizada
    /// </summary>
    public class CleanArchitectureProfile : Profile
    {
        public CleanArchitectureProfile()
        {
            // 📁 CATEGORY MAPPINGS
            CreateCategoryMappings();
            
            // 📁 PRODUCT MAPPINGS  
            CreateProductMappings();
            
            // 📁 USER MAPPINGS
            CreateUserMappings();
        }

        private void CreateCategoryMappings()
        {
            // Domain Entity → DTO
            CreateMap<DomainCategory, CategoryDto>()
                .ForMember(dest => dest.FormattedCreationDate, 
                    opt => opt.MapFrom(src => src.CreationDate.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.ProductCount, opt => opt.Ignore()); // Se llena en casos de uso

            // DTO → Domain Entity (para crear)
            CreateMap<CreateCategoryDto, DomainCategory>()
                .ConvertUsing((src, dest, context) => DomainCategory.Create(src.Name));

            // DTO → Domain Entity (para actualizar)
            CreateMap<UpdateCategoryDto, DomainCategory>()
                .ConvertUsing((src, dest, context) => DomainCategory.Create(src.Name));
        }

        private void CreateProductMappings()
        {
            // Domain Entity → DTO
            CreateMap<DomainProduct, ProductDto>()
                .ForMember(dest => dest.IsInStock, 
                    opt => opt.MapFrom(src => src.IsInStock()))
                .ForMember(dest => dest.StockStatus, 
                    opt => opt.MapFrom(src => src.Stock > 10 ? "En Stock" : src.Stock > 0 ? "Poco Stock" : "Sin Stock"))
                .ForMember(dest => dest.FormattedPrice, 
                    opt => opt.MapFrom(src => src.Price.ToString("C")))
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore()); // Se llena en casos de uso

            // DTO → Domain Entity (para crear)
            CreateMap<CreateProductDto, DomainProduct>()
                .ConvertUsing((src, dest, context) => 
                    DomainProduct.Create(
                        src.Name,
                        src.Description,
                        src.Price,
                        src.SKU,
                        src.Stock,
                        src.CategoryId,
                        src.ImgUrl,
                        src.ImgUrlLocal
                    ));

            // DTO → Domain Entity (para actualizar) - Se maneja en casos de uso
            CreateMap<UpdateProductDto, DomainProduct>()
                .ConvertUsing((src, dest, context) =>
                    DomainProduct.Create(
                        src.Name,
                        src.Description,
                        src.Price,
                        src.SKU,
                        src.Stock,
                        src.CategoryId,
                        src.ImgUrl,
                        src.ImgUrlLocal
                    ));
        }

        private void CreateUserMappings()
        {
            // Domain Entity → DTO
            CreateMap<DomainUser, UserDto>()
                .ForMember(dest => dest.Email, opt => opt.Ignore()); // Email está en Identity

            // DTO → Domain Entity (para crear)
            CreateMap<CreateUserDto, DomainUser>()
                .ConvertUsing((src, dest, context) => 
                    DomainUser.Create(src.Name, src.UserName, src.Role));

            // Mapeo para UserDataDto (información completa)
            CreateMap<DomainUser, UserDataDto>()
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // Se llena desde Identity
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Se llena desde Identity
        }
    }

    /// <summary>
    /// Profile específico para mapeos de Identity
    /// ✅ Separado porque maneja entidades de Identity
    /// </summary>
    public class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            // 🔄 MAPPINGS SIMPLIFICADOS PARA CLEAN ARCHITECTURE
            // Nota: En Clean Architecture ideal, no deberías mapear directamente desde Identity
            // Este es un mapping temporal para compatibilidad
        }
    }

    /// <summary>
    /// Profile para mapeos complejos que requieren lógica adicional
    /// ✅ Para casos donde necesitamos configuración especial
    /// </summary>
    public class CustomMappingProfile : Profile
    {
        public CustomMappingProfile()
        {
            // Mapeo personalizado para CategoryWithProductsDto
            CreateMap<DomainCategory, CategoryWithProductsDto>()
                .ForMember(dest => dest.Products, opt => opt.Ignore()); // Se llena en casos de uso
        }
    }
}