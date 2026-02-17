using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Application.Clients.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Common.Validation;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Clients.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClientService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClientResponseDto> CreateAsync(CreateClientDto dto)
        {
            await ClientValidator.ValidateCreateAsync(dto, _context);

            var client = _mapper.Map<Client>(dto);

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClientResponseDto>(client);
        }

        public async Task<ClientResponseDto> UpdateAsync(Guid id, UpdateClientDto dto)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new ValidationException("Cliente no encontrado.");

            await ClientValidator.ValidateCreateAsync(
                new CreateClientDto
                {
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    WorkSchedule = dto.WorkSchedule,
                    ChainId = dto.ChainId,
                    Extension = dto.Extension
                }, _context);

            _mapper.Map(dto, client);

            await _context.SaveChangesAsync();

            return _mapper.Map<ClientResponseDto>(client);
        }

        public async Task<List<ClientResponseDto>> GetAllAsync()
        {
            var clients = await _context.Clients.ToListAsync();
            return _mapper.Map<List<ClientResponseDto>>(clients);
        }
    }

}
