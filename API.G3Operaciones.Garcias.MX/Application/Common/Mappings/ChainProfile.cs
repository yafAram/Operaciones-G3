using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients; // O la ruta donde esté tu entidad Chain
using AutoMapper;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Mappings
{
    public class ChainProfile : Profile
    {
        public ChainProfile()
        {
            // Mapeo básico para el GetAllAsync y CreateAsync
            CreateMap<Chain, ChainResponseDto>();

            // Mapeo para los detalles de la cadena (GetDetailByIdAsync)
            // Aquí puedes configurar cómo se mapean las propiedades si difieren en nombre o estructura
            CreateMap<Chain, ChainDetailDto>()
                .ForMember(dest => dest.ChainId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ChainName, opt => opt.MapFrom(src => src.Name));

            // También puedes agregar el mapeo para CreateChainDto a Chain si lo necesitas en el futuro
            // CreateMap<CreateChainDto, Chain>();
        }
    }
}