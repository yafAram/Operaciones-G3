using API.G3Operaciones.Garcias.MX.Application.Inventory.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Orders.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Pricing.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Orders.Services
{
    public class OrderService : IOrderService, IOrderStatusUpdater, IOperatorAssignmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPriceConfigurationService _priceService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IAuditService _auditService;

        public OrderService(
            ApplicationDbContext context,
            IPriceConfigurationService priceService,
            IProductService productService,
            IWarehouseService warehouseService,
            IAuditService auditService)
        {
            _context = context;
            _priceService = priceService;
            _productService = productService;
            _warehouseService = warehouseService;
            _auditService = auditService;
        }

        // ================================
        // 🔹 CREATE ORDER (TRANSACCIONAL)
        // ================================
        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto request, string userId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            // 1. VALIDAR STOCK ANTES DE CREAR ORDEN
            foreach (var item in request.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(x =>
                        x.ProductId == item.ProductId &&
                        x.WarehouseId == request.WarehouseId);

                var available = inventory?.Quantity ?? 0;

                if (available < item.Quantity)
                    throw new Exception($"Stock insuficiente para producto {item.ProductId}. Disponible: {available}");
            }

            // 2. CREAR ORDEN
            var order = new Order(request.OperatorId, request.ClientId, request.ChainId);

            foreach (var item in request.Items)
            {
                var price = await _priceService.ResolvePriceAsync(
                    item.ProductId,
                    request.ChainId,
                    request.ClientId);

                order.AddItem(item.ProductId, item.Quantity, price);
            }

            order.CalculateTotal();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3. DESCONTAR INVENTARIO (YA VALIDADO)
            foreach (var item in request.Items)
            {
                await _productService.AdjustStockAsync(
                    new AdjustStockDto
                    {
                        ProductId = item.ProductId,
                        WarehouseId = request.WarehouseId,
                        Quantity = item.Quantity,
                        Reference = $"ORDER-{order.Id}"
                    },
                    MovementType.Out);
            }

            await _auditService.LogActionAsync(nameof(Order), order.Id, "Create", request, userId);

            await tx.CommitAsync();

            return Map(order);
        }

        // ================================
        // 🔹 EDIT ORDER
        // ================================
        public async Task<OrderResponseDto> EditOrderAsync(Guid orderId, EditOrderDto dto, string userId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var order = await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == orderId)
                ?? throw new Exception("Orden no encontrada");

            // Revertir inventario
            foreach (var item in order.Items)
            {
                await _productService.AdjustStockAsync(
                    new AdjustStockDto { ProductId = item.ProductId, Quantity = item.Quantity },
                    MovementType.In);
            }

            _context.RemoveRange(order.Items);

            foreach (var item in dto.Items)
            {
                var price = await _priceService.ResolvePriceAsync(item.ProductId, order.ChainId, order.ClientId);

                order.AddItem(item.ProductId, item.Quantity, price);

                await _productService.AdjustStockAsync(
                    new AdjustStockDto { ProductId = item.ProductId, Quantity = item.Quantity },
                    MovementType.Out);
            }

            order.CalculateTotal();

            await _context.SaveChangesAsync();
            await _auditService.LogActionAsync(nameof(Order), order.Id, "Edit", dto, userId);

            await tx.CommitAsync();

            return Map(order);
        }

        // ================================
        // 🔹 CANCEL ORDER
        // ================================
        public async Task CancelOrderAsync(Guid orderId, string userId)
        {
            var order = await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == orderId)
                ?? throw new Exception("Orden no encontrada");

            order.CancelOrder();

            foreach (var item in order.Items)
            {
                await _productService.AdjustStockAsync(
                    new AdjustStockDto { ProductId = item.ProductId, Quantity = item.Quantity },
                    MovementType.In);
            }

            await _context.SaveChangesAsync();
            await _auditService.LogActionAsync(nameof(Order), order.Id, "Cancel", null!, userId);
        }

        // ================================
        // 🔥 DELIVERY BRIDGE
        // ================================
        public async Task MarkAsDeliveredAsync(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order no encontrada");

            order.MarkAsDelivered();
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsIncidentAsync(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order no encontrada");

            order.MarkAsIncident();
            await _context.SaveChangesAsync();
        }

        // ================================
        // 📊 DASHBOARD ADMIN
        // ================================
        public async Task<AdminDashboardDto> GetAdminDashboardAsync(DateTime date)
        {
            var query = _context.Orders.AsNoTracking()
                .Where(o => o.CreatedAt.Date == date.Date);

            var orders = await query
                .Select(o => new OrderDto(
                    o.Id,
                    o.OperatorId,
                    o.ClientId,
                    o.ChainId,
                    o.TotalAmount,
                    o.Status,
                    o.CreatedAt))
                .ToListAsync();

            return new AdminDashboardDto(
                orders,
                orders.Count(x => x.Status == OrderStatus.Pending),
                orders.Count(x => x.Status == OrderStatus.Finalized),
                orders.Count(x => x.Status == OrderStatus.Cancelled)
            );
        }

        // ================================
        // 📊 DASHBOARD OPERADOR
        // ================================
        public async Task<OperatorDashboardDto> GetOperatorDashboardAsync(Guid operatorId, DateTime date)
        {
            var stops = await _context.DeliveryStops
                .AsNoTracking()
                .Where(s =>
                    s.DeliveryPlan.DeliveryUserId == operatorId &&
                    s.DeliveryPlan.Date == date.Date)
                .ToListAsync();

            var pending = stops
                .Where(x => x.Status == DeliveryStopStatus.Pending)
                .Select(x => new VisitDto(
                    x.Id,
                    x.ClientId,
                    null,
                    VisitState.Pending,
                    x.CreatedAt))
                .ToList();

            var delivered = await _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.OperatorId == operatorId &&
                    o.CreatedAt.Date == date.Date &&
                    !_context.DeliveryStops.Any(s =>
                        s.OrderId == o.Id &&
                        s.Status == DeliveryStopStatus.Delivered))
                .Select(o => new OrderDto(
                    o.Id,
                    o.OperatorId,
                    o.ClientId,
                    o.ChainId,
                    o.TotalAmount,
                    o.Status,
                    o.CreatedAt))
                .ToListAsync();

            return new OperatorDashboardDto(pending, new(), new(), delivered);
        }

        // ================================
        // 📊 HISTÓRICO + MÉTRICAS
        // ================================
        public async Task<List<OrderChainGroupDto>> GetAllGroupedOrdersAsync(DateTime? date = null)
        {
            var query = BaseOrdersQuery();

            if (date.HasValue)
                query = query.Where(o => o.CreatedAt.Date == date.Value.Date);

            var result = await (
                from o in query
                join c in _context.Clients on o.ClientId equals c.Id into gj
                from c in gj.DefaultIfEmpty()
                select new OrderChainGroupDto
                {
                    ChainId = o.ChainId,
                    ChainName = c != null ? c.Name : "Sin cadena",
                    TotalOrders = 1,
                    TotalAmount = o.TotalAmount
                }
            ).ToListAsync();

            return result;
        }

        // ================================
        // 🔥 LOGISTICS (CON JOIN DIRECTO)
        // ================================
        public async Task<LogisticsViewDto> GetLogisticsViewAsync()
        {
            var orders = await BaseOrdersQuery()
                .Select(o => new { o.Id, o.ClientId })
                .ToListAsync();

            var clients = await _context.Clients
                .AsNoTracking()
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ChainId,
                    ChainName = c.Chain != null ? c.Chain.Name : null
                })
                .ToListAsync();

            var operators = await _context.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .Where(x => x.Role.Name == "Operador")
                .Select(x => new LogisticsOperatorDto
                {
                    OperatorId = x.Id,
                    OperatorName = x.FirstName + " " + x.LastName
                })
                .ToListAsync();

            var allUsersMap = await _context.Users
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.FirstName + " " + x.LastName);

            var activeTargets = await (
                from t in _context.Set<OperatorAssignmentTarget>()
                join p in _context.Set<OperatorAssignmentPlan>()
                    on t.OperatorAssignmentPlanId equals p.Id
                where t.IsActive && !p.IsLocked
                select new
                {
                    t.ClientId,
                    t.ChainId,
                    p.OperatorId,
                    p.Date
                }
            ).ToListAsync();

            var clientToOperator = new Dictionary<Guid, Guid>();

            foreach (var target in activeTargets.OrderBy(x => x.Date))
            {
                if (target.ClientId.HasValue)
                    clientToOperator[target.ClientId.Value] = target.OperatorId;

                if (target.ChainId.HasValue)
                {
                    var chainClients = clients
                        .Where(c => c.ChainId == target.ChainId.Value)
                        .Select(c => c.Id);

                    foreach (var clientId in chainClients)
                        clientToOperator[clientId] = target.OperatorId;
                }
            }

            var ordersByClient = orders
                .Where(o => o.ClientId.HasValue)
                .GroupBy(o => o.ClientId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            var chainGroups = clients
                .Where(c => c.ChainId.HasValue)
                .GroupBy(c => c.ChainId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var allChainIds = chainGroups.Keys.ToHashSet();
            var resultChains = new List<LogisticsChainDto>();

            foreach (var kvp in chainGroups)
            {
                var chainId = kvp.Key;
                var branchList = kvp.Value;
                var clientIds = branchList.Select(x => x.Id).ToHashSet();

                resultChains.Add(new LogisticsChainDto
                {
                    ChainId = chainId,
                    ChainName = branchList.FirstOrDefault()?.ChainName ?? "Cadena",
                    TotalOrders = orders.Count(o =>
                        o.ClientId.HasValue &&
                        clientIds.Contains(o.ClientId.Value)),

                    Branches = branchList.Select(c =>
                    {
                        ordersByClient.TryGetValue(c.Id, out var orderCount);
                        clientToOperator.TryGetValue(c.Id, out var operatorId);

                        return new LogisticsBranchDto
                        {
                            ClientId = c.Id,
                            ClientName = c.Name,
                            TotalOrders = orderCount,
                            AssignedOperatorId = operatorId != Guid.Empty ? operatorId : null,
                            AssignedOperatorName = operatorId != Guid.Empty
                                ? allUsersMap.GetValueOrDefault(operatorId)
                                : null
                        };
                    }).ToList()
                });
            }

            var emptyChains = await _context.Set<Chain>()
                .AsNoTracking()
                .Where(c => !allChainIds.Contains(c.Id))
                .Select(c => new LogisticsChainDto
                {
                    ChainId = c.Id,
                    ChainName = c.Name,
                    TotalOrders = 0,
                    Branches = new List<LogisticsBranchDto>()
                })
                .ToListAsync();

            resultChains.AddRange(emptyChains);

            var independentClients = clients.Where(c => !c.ChainId.HasValue).ToList();

            if (independentClients.Any())
            {
                resultChains.Add(new LogisticsChainDto
                {
                    ChainId = null,
                    ChainName = "Sin cadena",
                    TotalOrders = orders.Count(o =>
                        o.ClientId.HasValue &&
                        independentClients.Select(c => c.Id).Contains(o.ClientId.Value)),

                    Branches = independentClients.Select(c =>
                    {
                        ordersByClient.TryGetValue(c.Id, out var orderCount);
                        clientToOperator.TryGetValue(c.Id, out var operatorId);

                        return new LogisticsBranchDto
                        {
                            ClientId = c.Id,
                            ClientName = c.Name,
                            TotalOrders = orderCount,
                            AssignedOperatorId = operatorId != Guid.Empty ? operatorId : null,
                            AssignedOperatorName = operatorId != Guid.Empty
                                ? allUsersMap.GetValueOrDefault(operatorId)
                                : null
                        };
                    }).ToList()
                });
            }

            return new LogisticsViewDto
            {
                Operators = operators,
                Chains = resultChains
            };
        }

        // ================================
        // 🔥 ASIGNACIONES (INSERCIÓN EXPLÍCITA Y VALIDACIÓN)
        // ================================
        public async Task AssignTargetsAsync(AssignOperatorTargetsDto dto, string adminUserId)
        {
            // 🛑 FRENO DE EMERGENCIA NUCLEAR:
            // Si ves este error en tu consola de Vue o en Swagger, significa que el frontend 
            // está mandando las variables como "clientId" en lugar de "clientIds: []".
            // Y por eso la BD quedaba en blanco.
            if ((dto.ClientIds == null || !dto.ClientIds.Any()) &&
                (dto.ChainIds == null || !dto.ChainIds.Any()))
            {
                throw new Exception("🛑 ERROR CRÍTICO: El backend recibió la petición, pero 'ClientIds' y 'ChainIds' están vacíos o nulos. ¡El frontend (Vue) o el payload enviado no tiene el formato correcto de arreglo!");
            }

            var plan = await _context.Set<OperatorAssignmentPlan>()
                .FirstOrDefaultAsync(x => x.OperatorId == dto.OperatorId && !x.IsLocked);

            if (plan == null)
            {
                plan = new OperatorAssignmentPlan(dto.OperatorId, DateTime.UtcNow.Date);
                _context.Set<OperatorAssignmentPlan>().Add(plan);
                await _context.SaveChangesAsync(); // <-- GUARDAMOS PARA GENERAR EL PLAN ID REAL
            }

            // Desactivamos targets previos
            var allActiveTargets = await _context.Set<OperatorAssignmentTarget>()
                .Where(t => t.IsActive)
                .ToListAsync();

            if (dto.ClientIds?.Any() == true)
            {
                foreach (var clientId in dto.ClientIds)
                {
                    // Apagamos los anteriores
                    var oldTargets = allActiveTargets.Where(t => t.ClientId == clientId).ToList();
                    foreach (var oldTarget in oldTargets)
                    {
                        oldTarget.Deactivate();
                        _context.Set<OperatorAssignmentTarget>().Update(oldTarget);
                    }

                    // 🔥 FUERZA BRUTA: INSERCIÓN DIRECTA A LA TABLA
                    var newTarget = new OperatorAssignmentTarget(plan.Id, clientId, null);
                    _context.Set<OperatorAssignmentTarget>().Add(newTarget);
                }
            }

            if (dto.ChainIds?.Any() == true)
            {
                foreach (var chainId in dto.ChainIds)
                {
                    // Apagamos los anteriores
                    var oldTargets = allActiveTargets.Where(t => t.ChainId == chainId).ToList();
                    foreach (var oldTarget in oldTargets)
                    {
                        oldTarget.Deactivate();
                        _context.Set<OperatorAssignmentTarget>().Update(oldTarget);
                    }

                    // 🔥 FUERZA BRUTA: INSERCIÓN DIRECTA A LA TABLA
                    var newTarget = new OperatorAssignmentTarget(plan.Id, null, chainId);
                    _context.Set<OperatorAssignmentTarget>().Add(newTarget);
                }
            }

            // AHORA SÍ GUARDAMOS LOS TARGETS EN LA BD
            await _context.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetAssignedClientIdsAsync(Guid operatorId)
        {
            var activeTargets = await (
                from t in _context.Set<OperatorAssignmentTarget>()
                join p in _context.Set<OperatorAssignmentPlan>() on t.OperatorAssignmentPlanId equals p.Id
                where t.IsActive && !p.IsLocked && p.OperatorId == operatorId
                select new { t.ClientId, t.ChainId }
            ).ToListAsync();

            var clientIds = activeTargets
                .Where(x => x.ClientId.HasValue)
                .Select(x => x.ClientId!.Value)
                .ToList();

            var chainIds = activeTargets
                .Where(x => x.ChainId.HasValue)
                .Select(x => x.ChainId!.Value)
                .ToList();

            if (chainIds.Any())
            {
                var chainClients = await _context.Clients
                    .Where(c => c.ChainId.HasValue && chainIds.Contains(c.ChainId.Value))
                    .Select(c => c.Id)
                    .ToListAsync();

                clientIds.AddRange(chainClients);
            }

            return clientIds.Distinct().ToList();
        }

        // ================================
        // 🔹 GET DETAIL
        // ================================
        public async Task<OrderResponseDto> GetOrderDetailsAsync(Guid orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == orderId)
                ?? throw new Exception("Orden no encontrada");

            return Map(order);
        }

        // ================================
        // 🔹 HELPERS
        // ================================
        private static OrderResponseDto Map(Order order)
        {
            return new OrderResponseDto(
                order.Id,
                order.TotalAmount,
                order.Status,
                order.TicketPath,
                order.CreatedAt,
                new List<string>());
        }


        private static IQueryable<Order> ApplyOrderVisibilityFilter(IQueryable<Order> query, bool includeDelivered)
        {
            if (includeDelivered)
                return query;

            return query.Where(o => o.Status != OrderStatus.Delivered);
        }

        private async Task<HashSet<Guid>> GetDeliveredOrderIdsAsync()
        {
            return (await _context.DeliveryStops
                .AsNoTracking()
                .Where(s => s.Status == DeliveryStopStatus.Delivered)
                .Select(s => s.OrderId)
                .Distinct()
                .ToListAsync())
                .ToHashSet();
        }

        private IQueryable<Order> BaseOrdersQuery(bool includeClosed = false)
        {
            return _context.Orders.AsNoTracking()
                .Where(o => includeClosed
                    || !_context.DeliveryStops.Any(s =>
                        s.OrderId == o.Id &&
                        s.Status == DeliveryStopStatus.Delivered));
        }

        public async Task MarkStopAsDeliveredAsync(Guid stopId)
        {
            var stop = await _context.DeliveryStops
                .FirstOrDefaultAsync(x => x.Id == stopId)
                ?? throw new KeyNotFoundException("Parada de entrega no encontrada.");

            stop.MarkAsDelivered();

            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == stop.OrderId);

            if (order != null)
                order.MarkAsDelivered();

            await _context.SaveChangesAsync();
        }

        // ================================
        // ❌ DEPRECATED
        // ================================
        public Task ConfigureScheduleAsync(ConfigureScheduleDto dto, string adminUserId)
            => Task.CompletedTask;

        public Task<Guid> AssignVisitAsync(AssignVisitDto dto, string adminUserId)
            => throw new NotImplementedException("ClientVisit eliminado. Usa DeliveryPlan.");

        public Task UpdateVisitStateAsync(Guid visitId, UpdateVisitStateDto dto, string currentUserId)
            => throw new NotImplementedException("Ahora se usa DeliveryService.");
    }
}