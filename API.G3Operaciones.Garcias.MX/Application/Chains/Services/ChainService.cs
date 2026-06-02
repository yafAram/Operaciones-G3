using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Application.Chains.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Application.Common.Validation;
using API.G3Operaciones.Garcias.MX.Application.Orders.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.G3Operaciones.Garcias.MX.Application.Chains.Services
{
    public class ChainService : IChainService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public ChainService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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


        public async Task<List<ChainResponseDto>> GetAllAsync()
        {
            var chains = await _context.Chains.ToListAsync();
            // Ahora que existe ChainProfile, esto ya no lanzará excepción
            return _mapper.Map<List<ChainResponseDto>>(chains);
        }


        public async Task DeactivateAsync(Guid id)
        {
            var chain = await _context.Chains
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new ValidationException("Cadena no encontrado.");

            chain.Deactivate();

            await _context.SaveChangesAsync();
        }



        public async Task<ChainDetailDto> GetDetailByIdAsync(Guid chainId)
        {
            var chain = await _context.Chains
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == chainId)
                ?? throw new ValidationException("Cadena no encontrada.");

            var dto = _mapper.Map<ChainDetailDto>(chain);

            var branches = await _context.Clients
                .AsNoTracking()
                .Where(c => c.ChainId == chainId)
                .Select(c => new BranchSummaryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    IsActive = c.IsActive,
                    Extension = c.Extension,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    TotalOrders = 0,     // пока no tienes órdenes
                    TotalSales = 0
                })
                .ToListAsync();

            dto.Branches = branches;
            dto.TotalBranches = branches.Count;

            return dto;
        }



    }
    }
