using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using AutoMapper;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Mappings
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            // Response básico
            CreateMap<Client, ClientResponseDto>();

            // Detail
            CreateMap<Client, ClientDetailDto>()
                .ForMember(dest => dest.Chain,
                    opt => opt.MapFrom(src => src.Chain == null
                        ? null
                        : new ChainInfoDto
                        {
                            Id = src.Chain.Id,
                            Name = src.Chain.Name
                        }));

            // Create → usar constructor explícito
            CreateMap<CreateClientDto, Client>()
                .ConstructUsing(src => new Client(
                    src.Name,
                    src.PhoneNumber,
                    src.Address,
                    src.WorkSchedule,
                    src.ChainId,
                    src.Extension,
                    src.Latitude,
                    src.Longitude
                ));

            CreateMap<UpdateClientDto, Client>();
        }
    }
}
