using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Application.Clients.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Common.Validation;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;

namespace API.G3Operaciones.Garcias.MX.Application.Clients.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClientResponseDto> CreateAsync(CreateClientDto dto)
        {
            await ClientValidator.ValidateCreateAsync(dto, _context);

            var client = new Client(
                dto.Name,
                dto.PhoneNumber,
                dto.Address,
                dto.WorkSchedule,
                dto.ChainId,
                dto.Extension
            );

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return new ClientResponseDto
            {
                Id = client.Id,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                WorkSchedule = client.WorkSchedule,
                ChainId = client.ChainId,
                Extension = client.Extension,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt
            };
        }
    }
}
