using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthNet.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
        }
    }
}
