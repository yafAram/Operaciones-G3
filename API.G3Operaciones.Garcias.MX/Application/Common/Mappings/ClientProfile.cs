using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using AutoMapper;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Mappings
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientResponseDto>();

            CreateMap<CreateClientDto, Client>();

            CreateMap<UpdateClientDto, Client>();
        }
    }
}
