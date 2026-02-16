using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Application.Chains.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Common.Validation;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;

namespace API.G3Operaciones.Garcias.MX.Application.Chains.Services
{
    public class ChainService : IChainService
    {
        private readonly ApplicationDbContext _context;

        public ChainService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChainResponseDto> CreateAsync(CreateChainDto dto)
        {
            await ChainValidator.ValidateCreateAsync(dto, _context);

            var chain = new Chain(dto.Name, dto.BasePhoneNumber);

            _context.Chains.Add(chain);
            await _context.SaveChangesAsync();

            return new ChainResponseDto
            {
                Id = chain.Id,
                Name = chain.Name,
                BasePhoneNumber = chain.BasePhoneNumber,
                IsActive = chain.IsActive,
                CreatedAt = chain.CreatedAt
            };
        }
    }
    }
