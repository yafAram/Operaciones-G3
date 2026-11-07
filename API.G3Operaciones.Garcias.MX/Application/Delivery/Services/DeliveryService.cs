using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Delivery.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRouteOptimizationService _routeService;
        private readonly IGoogleMapsService _mapsService;
        private readonly IPdfTicketService _pdfTicketService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeliveryService(
            ApplicationDbContext context,
            IRouteOptimizationService routeService,
            IGoogleMapsService mapsService,
            IPdfTicketService pdfTicketService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _routeService = routeService;
            _mapsService = mapsService;
            _httpContextAccessor = httpContextAccessor;
            _pdfTicketService = pdfTicketService;
        }

        // =========================
        // PLANES (ADMIN)
        // =========================

        public async Task<Guid> CreatePlanAsync(CreateDeliveryAssignmentPlanDto dto)
        {
            var existingPlan = await _context.DeliveryAssignmentPlans
                .FirstOrDefaultAsync(x =>
                    x.DeliveryUserId == dto.DeliveryUserId &&
                    x.DeliveryDate.Date == dto.DeliveryDate.Date);

            if (existingPlan != null)
                return existingPlan.Id;

            var plan = new DeliveryAssignmentPlan(dto.DeliveryUserId, dto.DeliveryDate);

            _context.DeliveryAssignmentPlans.Add(plan);
            await _context.SaveChangesAsync();

            return plan.Id;
        }

        public async Task<List<DeliveryPlanSummaryDto>> GetPlansAsync(DateTime date)
        {
            return await _context.DeliveryAssignmentPlans
                .AsNoTracking()
                .Include(x => x.Targets)
                .Where(x => x.DeliveryDate.Date == date.Date)
                .Select(x => new DeliveryPlanSummaryDto
                {
                    Id = x.Id,
                    DeliveryUserId = x.DeliveryUserId,
                    Date = x.DeliveryDate,
                    Status = x.IsLocked ? "CONFIRMED" : "PENDING",
                    TotalStops = x.Targets.Count(t => t.IsActive),
                    DeliveredStops = 0,
                    PendingStops = x.Targets.Count(t => t.IsActive),
                    FailedStops = 0
                })
                .ToListAsync();
        }

        public async Task<DeliveryPlanDetailDto> GetPlanDetailAsync(Guid planId)
        {
            var plan = await _context.DeliveryAssignmentPlans
                .AsNoTracking()
                .Include(x => x.Targets)
                .FirstOrDefaultAsync(x => x.Id == planId)
                ?? throw new Exception("Plan no encontrado");

            // 🔥 CORRECCIÓN: Filtramos solo los targets activos
            var activeTargets = plan.Targets.Where(t => t.IsActive).ToList();
            var clientIds = await ResolveClientIds(activeTargets);

            var clients = await _context.Clients
                .Where(c => clientIds.Contains(c.Id))
                .ToListAsync();

            // 🔥 CORRECCIÓN: Relacionamos el Cliente con su TargetId para mandarlo al frontend
            var stops = clients.Select(c =>
            {
                var target = activeTargets.FirstOrDefault(t =>
                    t.ClientId == c.Id ||
                    (c.ChainId.HasValue && t.ChainId == c.ChainId.Value));

                return new DeliveryStopDto
                {
                    TargetId = target?.Id, // <-- ESTO SOLUCIONA EL ERROR AL ELIMINAR
                    ClientId = c.Id,
                    ClientName = c.Name,
                    Address = c.Address
                };
            }).ToList();

            return new DeliveryPlanDetailDto
            {
                Id = plan.Id,
                DeliveryUserId = plan.DeliveryUserId,
                Date = plan.DeliveryDate,
                Status = plan.IsLocked ? "CONFIRMED" : "PENDING",
                Stops = stops
            };
        }

        public async Task DeletePlanAsync(Guid planId)
        {
            var plan = await _context.DeliveryAssignmentPlans
                .Include(x => x.Targets)
                .FirstOrDefaultAsync(x => x.Id == planId)
                ?? throw new Exception("Plan no encontrado");

            _context.DeliveryAssignmentTargets.RemoveRange(plan.Targets);
            _context.DeliveryAssignmentPlans.Remove(plan);

            await _context.SaveChangesAsync();
        }

        // =========================
        // TARGETS
        // =========================



        public async Task<Guid> AddTargetAsync(Guid? planId, AddDeliveryAssignmentTargetDto dto)
        {
            var deliveryUserId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(deliveryUserId, out var userId))
                throw new UnauthorizedAccessException("Token inválido.");

            var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Administrador") == true;

            if (dto.ChainId == null && dto.ClientId == null)
                throw new InvalidOperationException("Debe enviar ChainId o ClientId.");

            var plan = await _context.DeliveryAssignmentPlans
                .Include(x => x.Targets)
                .FirstOrDefaultAsync(x =>
                    (planId != null && x.Id == planId &&
                     (isAdmin || x.DeliveryUserId == userId)) ||
                    (planId == null && !isAdmin &&
                     x.DeliveryUserId == userId &&
                     x.DeliveryDate.Date == DateTime.UtcNow.Date));

            if (plan == null)
            {
                plan = new DeliveryAssignmentPlan(userId, DateTime.UtcNow.Date);
                _context.DeliveryAssignmentPlans.Add(plan);
                await _context.SaveChangesAsync();
            }

            if (plan.IsLocked)
                throw new InvalidOperationException("No se pueden modificar asignaciones de un plan confirmado.");

            var exists = await _context.DeliveryAssignmentTargets.AnyAsync(t =>
                t.DeliveryAssignmentPlanId == plan.Id &&
                t.IsActive &&
                t.ClientId == dto.ClientId &&
                t.ChainId == dto.ChainId);

            if (exists)
                throw new InvalidOperationException("Ya existe este target.");

            var target = new DeliveryAssignmentTarget(plan.Id, dto.ChainId, dto.ClientId);

            plan.AddTarget(target);
            _context.DeliveryAssignmentTargets.Add(target);

            await _context.SaveChangesAsync();

            var clientIds = await ResolveClientIds(
                await _context.DeliveryAssignmentTargets
                    .AsNoTracking()
                    .Where(t =>
                        t.DeliveryAssignmentPlanId == plan.Id &&
                        t.IsActive)
                    .ToListAsync()
            );

            if (clientIds.Count > 100)
                throw new InvalidOperationException("Máximo 100 sucursales por repartidor.");

            return target.Id;
        }

        public async Task RemoveTargetAsync(Guid targetId)
        {
            var target = await _context.DeliveryAssignmentTargets
                .Include(x => x.DeliveryAssignmentPlan)
                .FirstOrDefaultAsync(x => x.Id == targetId)
                ?? throw new Exception("Target no encontrado");

            var currentUser = _httpContextAccessor.HttpContext?.User;
            var isAdmin = currentUser?.IsInRole("Administrador") == true;
            var currentUserId = currentUser?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!isAdmin &&
                (!Guid.TryParse(currentUserId, out var userId) ||
                 target.DeliveryAssignmentPlan.DeliveryUserId != userId))
            {
                throw new UnauthorizedAccessException("No tienes permiso para modificar este plan.");
            }

            if (target.DeliveryAssignmentPlan.IsLocked)
                throw new InvalidOperationException("No se pueden modificar asignaciones de un plan confirmado.");

            target.Deactivate();
            await _context.SaveChangesAsync();
        }

        // =========================
        // OVERVIEW / DASHBOARD
        // =========================

        public async Task<List<DeliveryAssignmentOverviewDto>> GetDeliveryOverviewAsync(DateTime date)
        {
            var deliveredOrderIds = await _context.DeliveryStops
                .AsNoTracking()
                .Where(s => s.Status == DeliveryStopStatus.Delivered)
                .Select(s => s.OrderId)
                .Distinct()
                .ToHashSetAsync();

            var pendingOrders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o =>
                    o.Status == OrderStatus.Pending &&
                    o.ClientId.HasValue &&
                    !deliveredOrderIds.Contains(o.Id))
                .ToListAsync();

            if (!pendingOrders.Any())
                return new List<DeliveryAssignmentOverviewDto>();

            var clientIds = pendingOrders
                .Select(o => o.ClientId!.Value)
                .Distinct()
                .ToList();

            var clientsWithOrders = await _context.Clients
                .AsNoTracking()
                .Include(c => c.Chain)
                .Where(c => clientIds.Contains(c.Id))
                .ToListAsync();

            var todaysAssignments = await _context.DeliveryAssignmentTargets
                .AsNoTracking()
                .Include(t => t.DeliveryAssignmentPlan)
                .Where(t =>
                    t.IsActive &&
                    t.DeliveryAssignmentPlan.DeliveryDate.Date == date.Date)
                .ToListAsync();

            var driverIds = todaysAssignments
                .Select(t => t.DeliveryAssignmentPlan.DeliveryUserId)
                .Distinct()
                .ToList();

            var drivers = await _context.Users
                .AsNoTracking()
                .Where(u => driverIds.Contains(u.Id))
                .ToListAsync();

            var todaysStops = await _context.DeliveryStops
                .AsNoTracking()
                .Where(s =>
                    s.DeliveryPlan.Date.Date == date.Date &&
                    s.Status != DeliveryStopStatus.Delivered)
                .ToListAsync();

            var result = new List<DeliveryAssignmentOverviewDto>();

            foreach (var client in clientsWithOrders)
            {
                var clientOrders = pendingOrders
                    .Where(o => o.ClientId == client.Id)
                    .Where(o => !deliveredOrderIds.Contains(o.Id))
                    .ToList();

                var assignment = todaysAssignments.FirstOrDefault(t =>
                    t.ClientId == client.Id ||
                    (client.ChainId.HasValue && t.ChainId == client.ChainId.Value));

                var plan = assignment?.DeliveryAssignmentPlan;

                var driver = plan != null
                    ? drivers.FirstOrDefault(d => d.Id == plan.DeliveryUserId)
                    : null;

                var orderDtos = clientOrders.Select(o =>
                {
                    var stop = todaysStops.FirstOrDefault(s => s.OrderId == o.Id);

                    return new OrderSimpleDto
                    {
                        OrderId = o.Id,
                        DeliveryStopId = stop?.Id,
                        TotalAmount = o.TotalAmount,
                        CreatedAt = o.CreatedAt,
                        OrderStatus = o.Status.ToString(),
                        OfficialDeliveryStatus = stop?.Status.ToString() ?? "Unassigned",
                        DeliveryPlanStatus = stop?.Status.ToString(),
                        FailureReason = null,
                        HasIncident = stop?.Status == DeliveryStopStatus.Failed,
                        IsDelivered = stop?.Status == DeliveryStopStatus.Delivered,
                        Items = o.Items.Select(i => new OrderItemSimpleDto
                        {
                            ProductId = i.ProductId,
                            ProductName = i.Product?.Name ?? "Producto Desconocido",
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            SubTotal = i.Quantity * i.UnitPrice
                        }).ToList()
                    };
                }).ToList();

                result.Add(new DeliveryAssignmentOverviewDto
                {
                    PlanId = plan?.Id,
                    TargetId = assignment?.Id,
                    ChainId = client.ChainId,
                    ChainName = client.Chain?.Name,
                    ClientId = client.Id,
                    ClientName = client.Name,
                    Address = client.Address ?? "",
                    Latitude = (double?)client.Latitude,
                    Longitude = (double?)client.Longitude,
                    Phone = client.PhoneNumber,
                    BasePhone = client.Chain?.BasePhoneNumber,
                    Extension = client.Extension,
                    TotalAmount = clientOrders.Sum(o => o.TotalAmount),
                    OrdersCount = clientOrders.Count,
                    AssignedDeliveryUserId = plan?.DeliveryUserId,
                    AssignedDeliveryUserName = driver != null
                        ? $"{driver.FirstName} {driver.LastName}"
                        : null,
                    Orders = orderDtos
                });
            }

            return result
                .OrderByDescending(x => x.OrdersCount)
                .ToList();
        }

        // =========================
        // ROUTING
        // =========================

        public async Task<DeliveryRouteResponseDto> GenerateRouteAsync(GenerateRouteDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var planAssignment = await _context.DeliveryAssignmentPlans
                    .Include(x => x.Targets)
                    .FirstOrDefaultAsync(x =>
                        x.DeliveryUserId == dto.DeliveryUserId &&
                        x.DeliveryDate.Date == dto.Date.Date)
                    ?? throw new Exception("No existe plan.");

                var clientIds = await ResolveClientIds(planAssignment.Targets.ToList());

                if (!clientIds.Any())
                    throw new Exception("Sin clientes asignados.");

                var orders = await _context.Orders
                    .Where(o =>
                        o.Status == OrderStatus.Pending &&
                        o.ClientId.HasValue &&
                        clientIds.Contains(o.ClientId.Value))
                    .ToListAsync();

                var clients = await _context.Clients
                    .Where(c => clientIds.Contains(c.Id) &&
                                c.Latitude != null &&
                                c.Longitude != null)
                    .ToListAsync();

                var stops = (from o in orders
                             join c in clients on o.ClientId equals c.Id
                             select new DeliveryStopDto
                             {
                                 OrderId = o.Id,
                                 ClientId = c.Id,
                                 ClientName = c.Name,
                                 Address = c.Address,
                                 Latitude = (decimal)(double)c.Latitude!.Value,
                                 Longitude = (decimal)(double)c.Longitude!.Value
                             }).ToList();

                var optimized = _routeService.OptimizeRoute(
                    dto.CurrentLatitude,
                    dto.CurrentLongitude,
                    stops);

                var plan = new DeliveryPlan(dto.DeliveryUserId, dto.Date);

                foreach (var stop in optimized)
                {
                    plan.AddStop(new DeliveryStop(
                        plan.Id,
                        stop.OrderId,
                        stop.ClientId,
                        stop.Latitude,
                        stop.Longitude,
                        stop.Sequence,
                        stop.RouteGroup));
                }

                _context.DeliveryPlans.Add(plan);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DeliveryRouteResponseDto
                {
                    DeliveryPlanId = plan.Id,
                    Stops = await BuildStopDtosAsync(plan.Stops)
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DeliveryRouteResponseDto> StartRouteAsync(StartRouteDto dto)
        {
            // 1. Traer TODOS los planes del repartidor de hoy
            var today = DateTime.UtcNow.Date;

            var plans = await _context.DeliveryAssignmentPlans
    .Include(x => x.Targets)
    .Where(x =>
        x.DeliveryUserId == dto.DeliveryUserId &&
        x.DeliveryDate.Date == today)
    .ToListAsync();

            // 2. Filtrar solo planes útiles (con targets activos)
            var validPlan = plans
                .Where(p => p.Targets.Any(t => t.IsActive))
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            if (validPlan == null)
                throw new Exception("No tienes un plan válido asignado.");

            var activeTargets = validPlan.Targets
                .Where(t => t.IsActive)
                .ToList();

            var clientIds = await ResolveClientIds(activeTargets);

            if (!clientIds.Any())
                throw new Exception("El plan no tiene sucursales activas.");

            // 3. EXCLUIR órdenes ya entregadas REALMENTE (DeliveryStops manda)
            var deliveredOrderIds = await _context.DeliveryStops
                .AsNoTracking()
                .Where(s => s.Status == DeliveryStopStatus.Delivered)
                .Select(s => s.OrderId)
                .ToHashSetAsync();

            // 4. SOLO órdenes pendientes reales
            var pendingOrders = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o =>
                    o.ClientId.HasValue &&
                    clientIds.Contains(o.ClientId.Value) &&
                    !deliveredOrderIds.Contains(o.Id))
                .ToListAsync();

            if (!pendingOrders.Any())
                throw new Exception("No hay órdenes pendientes en este momento.");

            // 5. SI ya existe un DeliveryPlan operativo reutilizarlo
            var existingRoutePlan = await _context.DeliveryPlans
                .Include(x => x.Stops)
                .Where(x =>
                    x.DeliveryUserId == dto.DeliveryUserId &&
                    x.Date.Date == today &&
                    x.Status != DeliveryStatus.Completed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingRoutePlan != null && existingRoutePlan.Stops.Any())
            {
                existingRoutePlan.Start();
                await _context.SaveChangesAsync();

                return await BuildRouteResponseAsync(
                    existingRoutePlan,
                    dto.CurrentLatitude,
                    dto.CurrentLongitude
                );
            }

            // 6. CLIENTES
            var clients = await _context.Clients
                .Where(c => clientIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id);

            // 7. CREAR PLAN NUEVO SOLO SI ES NECESARIO
            var plan = new DeliveryPlan(
                dto.DeliveryUserId,
                today,
                (decimal)dto.CurrentLatitude,
                (decimal)dto.CurrentLongitude
            );

            _context.DeliveryPlans.Add(plan);
            await _context.SaveChangesAsync();

            // 8. ARMAR STOPS
            var stops = new List<DeliveryStopDto>();

            foreach (var order in pendingOrders)
            {
                clients.TryGetValue(order.ClientId!.Value, out var client);

                stops.Add(new DeliveryStopDto
                {
                    OrderId = order.Id,
                    ClientId = order.ClientId.Value,
                    ClientName = client?.Name ?? "",
                    Address = client?.Address ?? "",
                    Latitude = (decimal)(client?.Latitude ?? 0),
                    Longitude = (decimal)(client?.Longitude ?? 0)
                });
            }

            // 9. OPTIMIZAR
            var optimized = _routeService.OptimizeRoute(
                (decimal)dto.CurrentLatitude,
                (decimal)dto.CurrentLongitude,
                stops);

            // 10. PERSISTIR STOPS
            foreach (var s in optimized)
            {
                _context.DeliveryStops.Add(new DeliveryStop(
                    plan.Id,
                    s.OrderId,
                    s.ClientId,
                    s.Latitude,
                    s.Longitude,
                    s.Sequence,
                    s.RouteGroup
                ));
            }

            plan.Start();
            await _context.SaveChangesAsync();

            var saved = await _context.DeliveryPlans
                .Include(x => x.Stops)
                .FirstAsync(x => x.Id == plan.Id);

            return await BuildRouteResponseAsync(
                saved,
                dto.CurrentLatitude,
                dto.CurrentLongitude
            );
        }

        public async Task<DeliveryRouteResponseDto> GetCurrentRouteAsync(Guid deliveryUserId, DateTime date)
        {
            var activePlan = await _context.DeliveryPlans
                .Include(p => p.Stops)
                .Where(p => p.DeliveryUserId == deliveryUserId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            // SOLO PENDING EN PLAN ACTIVO
            if (activePlan != null && activePlan.Stops.Any())
            {
                var validStops = activePlan.Stops
                    .Where(s => s.Status == DeliveryStopStatus.Pending)
                    .OrderBy(s => s.Sequence)
                    .ToList();

                return new DeliveryRouteResponseDto
                {
                    DeliveryPlanId = activePlan.Id,
                    Stops = await BuildStopDtosAsync(validStops)
                };
            }

            var activeTargets = await _context.DeliveryAssignmentTargets
                .AsNoTracking()
                .Include(t => t.DeliveryAssignmentPlan)
                .Where(t =>
                    t.IsActive &&
                    t.DeliveryAssignmentPlan.DeliveryUserId == deliveryUserId &&
                    t.DeliveryAssignmentPlan.DeliveryDate.Date == date.Date)
                .ToListAsync();

            var clientIds = await ResolveClientIds(activeTargets);

            // excluir entregadas
            var deliveredOrderIds = await _context.DeliveryStops
                .AsNoTracking()
                .Where(s => s.Status == DeliveryStopStatus.Delivered)
                .Select(s => s.OrderId)
                .Distinct()
                .ToHashSetAsync();

            // excluir FAILED también (evita re-procesos raros)
            var failedOrderIds = await _context.DeliveryStops
                .AsNoTracking()
                .Where(s => s.Status == DeliveryStopStatus.Failed)
                .Select(s => s.OrderId)
                .Distinct()
                .ToHashSetAsync();

            var excludedOrderIds = deliveredOrderIds
                .Union(failedOrderIds)
                .ToHashSet();

            var pendingOrders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o =>
                    o.ClientId.HasValue &&
                    clientIds.Contains(o.ClientId.Value) &&
                    !excludedOrderIds.Contains(o.Id))
                .ToListAsync();

            if (!pendingOrders.Any())
            {
                return new DeliveryRouteResponseDto
                {
                    DeliveryPlanId = Guid.Empty,
                    Stops = new List<DeliveryStopDto>()
                };
            }

            var clients = await _context.Clients
                .AsNoTracking()
                .Where(c => clientIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id);

            int seq = 0;

            var virtualStops = pendingOrders
                .GroupBy(o => o.Id)
                .Select(g =>
                {
                    var order = g.First();
                    clients.TryGetValue(order.ClientId!.Value, out var client);

                    return new DeliveryStopDto
                    {
                        DeliveryStopId = Guid.Empty,
                        OrderId = order.Id,
                        ClientId = order.ClientId.Value,
                        ClientName = client?.Name ?? "",
                        Address = client?.Address ?? "",
                        Latitude = (decimal)(client?.Latitude ?? 0),
                        Longitude = (decimal)(client?.Longitude ?? 0),
                        Sequence = ++seq,
                        RouteGroup = 1,
                        Status = DeliveryStopStatus.Pending
                    };
                })
                .ToList();

            return new DeliveryRouteResponseDto
            {
                DeliveryPlanId = Guid.Empty,
                Stops = virtualStops
            };
        }

        // =========================
        // EJECUCIÓN
        // =========================

        public async Task CompleteDeliveryAsync(CompleteDeliveryDto dto, string userId)
        {
            var stop = await _context.DeliveryStops
                .Include(x => x.DeliveryPlan)
                .FirstOrDefaultAsync(x => x.Id == dto.DeliveryStopId)
                ?? throw new Exception("Stop no encontrado");

            stop.MarkAsDelivered();

            await _context.SaveChangesAsync();
        }

        public async Task ReportIncidentAsync(ReportIncidentDto dto, string userId)
        {
            var stop = await _context.DeliveryStops
                .Include(x => x.DeliveryPlan)
                .FirstOrDefaultAsync(x => x.Id == dto.DeliveryStopId)
                ?? throw new Exception("Stop no encontrado");

            if (string.IsNullOrWhiteSpace(dto.Notes))
                throw new InvalidOperationException("Debes indicar el motivo de la incidencia.");

            stop.MarkAsFailed(dto.Notes);

            await _context.SaveChangesAsync();
        }

        // =========================
        // CONSULTAS
        // =========================

        public async Task<List<RouteResponseDto>> GetRoutesAsync(DateTime date)
        {
            return await _context.DeliveryAssignmentPlans
                .Where(x => x.DeliveryDate.Date == date.Date)
                .Select(x => new RouteResponseDto(
                    x.Id,
                    x.DeliveryUserId,
                    "",
                    x.DeliveryDate,
                    x.IsLocked ? "CONFIRMED" : "PENDING",
                    x.Targets.Count(t => t.IsActive),
                    0,
                    0,
                    x.Targets.Count(t => t.IsActive)))
                .ToListAsync();
        }

        public async Task<List<DeliveryUserResponseDto>> GetDeliveryUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Role.Name == "Repartidor")
                .Select(u => new DeliveryUserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                })
                .ToListAsync();
        }

        public async Task ReassignDriverAsync(Guid deliveryPlanId, Guid deliveryUserId)
        {
            var plan = await _context.DeliveryPlans
                .FirstOrDefaultAsync(x => x.Id == deliveryPlanId)
                ?? throw new Exception("Plan no encontrado");

            _context.Entry(plan)
                .Property(nameof(DeliveryPlan.DeliveryUserId))
                .CurrentValue = deliveryUserId;

            await _context.SaveChangesAsync();
        }

        public async Task ConfirmPlanAsync(Guid planId)
        {
            var plan = await _context.DeliveryAssignmentPlans
                .Include(x => x.Targets)
                .FirstOrDefaultAsync(x => x.Id == planId)
                ?? throw new Exception("Plan no encontrado");

            var currentUser = _httpContextAccessor.HttpContext?.User;
            var isAdmin = currentUser?.IsInRole("Administrador") == true;
            var currentUserId = currentUser?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!isAdmin &&
                (!Guid.TryParse(currentUserId, out var userId) || plan.DeliveryUserId != userId))
            {
                throw new UnauthorizedAccessException("No tienes permiso para confirmar este plan.");
            }

            if (!plan.Targets.Any(target => target.IsActive))
                throw new InvalidOperationException("No puedes bloquear un plan sin asignaciones.");

            plan.Lock();
            await _context.SaveChangesAsync();
        }

        public async Task CancelRouteAsync(Guid deliveryPlanId)
        {
            var plan = await _context.DeliveryPlans
                .FirstOrDefaultAsync(x => x.Id == deliveryPlanId)
                ?? throw new Exception("Plan no encontrado");

            plan.Cancel();
            await _context.SaveChangesAsync();
        }

        // =========================
        // HELPERS
        // =========================

        private async Task<List<Guid>> ResolveClientIds(List<DeliveryAssignmentTarget> targets)
        {
            var direct = targets
                .Where(t => t.IsActive && t.ClientId.HasValue)
                .Select(t => t.ClientId!.Value)
                .ToList();

            var chains = targets
                .Where(t => t.IsActive && t.ChainId.HasValue)
                .Select(t => t.ChainId!.Value)
                .ToList();

            var chainClients = await _context.Clients
                .Where(c => c.ChainId.HasValue && chains.Contains(c.ChainId.Value))
                .Select(c => c.Id)
                .ToListAsync();

            return direct.Concat(chainClients).Distinct().ToList();
        }

        private async Task<List<DeliveryStopDto>> BuildStopDtosAsync(IEnumerable<DeliveryStop> stops)
        {
            var clientIds = stops.Select(x => x.ClientId).Distinct().ToList();

            var clients = await _context.Clients
                .Where(x => clientIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => new { x.Name, x.Address });

            return stops.Select(x =>
            {
                clients.TryGetValue(x.ClientId, out var c);

                return new DeliveryStopDto
                {
                    DeliveryStopId = x.Id,
                    OrderId = x.OrderId,
                    ClientId = x.ClientId,
                    ClientName = c?.Name ?? "",
                    Address = c?.Address ?? "",
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Sequence = x.Sequence,
                    RouteGroup = x.RouteGroup,
                    Status = x.Status
                };
            }).ToList();
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


        private async Task<DeliveryRouteResponseDto> BuildRouteResponseAsync(
      DeliveryPlan plan,
      double startLatitude,
      double startLongitude)
        {
            var stopOrderIds = plan.Stops.Select(s => s.OrderId).Distinct().ToList();
            var clientIds = plan.Stops.Select(s => s.ClientId).Distinct().ToList();

            var clients = await _context.Clients
                .AsNoTracking()
                .Where(c => clientIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id);

            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => stopOrderIds.Contains(o.Id))
                .ToDictionaryAsync(o => o.Id);

            var stops = plan.Stops
                .OrderBy(s => s.Sequence)
                .Select(s =>
                {
                    clients.TryGetValue(s.ClientId, out var client);
                    orders.TryGetValue(s.OrderId, out var order);

                    var items = order?.Items.Select(i => new OrderItemSimpleDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name ?? "Producto Desconocido",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SubTotal = i.Quantity * i.UnitPrice
                    }).ToList() ?? new List<OrderItemSimpleDto>();

                    return new DeliveryStopDto
                    {
                        DeliveryStopId = s.Id,
                        OrderId = s.OrderId,
                        ClientId = s.ClientId,
                        ClientName = client?.Name ?? "",
                        Address = client?.Address ?? "",
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        Sequence = s.Sequence,
                        RouteGroup = s.RouteGroup,
                        Status = s.Status,

                        OrderTotal = order?.TotalAmount ?? 0,
                        ItemsCount = items.Sum(x => x.Quantity),
                        Items = items
                    };
                })
                .ToList();

            var routeGroups = BuildRouteGroups(startLatitude, startLongitude, stops);

            return new DeliveryRouteResponseDto
            {
                DeliveryPlanId = plan.Id,
                Stops = stops,
                RouteGroups = routeGroups
            };
        }

        private List<RouteGroupDto> BuildRouteGroups(
    double startLatitude,
    double startLongitude,
    List<DeliveryStopDto> stops)
        {
            var links = _mapsService.GenerateRouteLinks(
                (decimal)startLatitude,
                (decimal)startLongitude,
                stops);

            var groups = stops
                .GroupBy(x => x.RouteGroup)
                .OrderBy(g => g.Key)
                .ToList();

            var result = new List<RouteGroupDto>();

            for (int i = 0; i < groups.Count; i++)
            {
                var groupStops = groups[i]
                    .OrderBy(x => x.Sequence)
                    .ToList();

                result.Add(new RouteGroupDto
                {
                    RouteGroup = groups[i].Key,
                    GoogleMapsUrl = links.ElementAtOrDefault(i) ?? "",
                    Stops = groupStops
                });
            }

            return result;
        }

        private IQueryable<Order> BaseOrdersQuery(bool includeDelivered = false)
        {
            var query = _context.Orders.AsNoTracking();

            if (includeDelivered)
                return query;

            // Excluir órdenes que YA fueron entregadas (según DeliveryStop)
            var deliveredOrderIds = _context.DeliveryStops
                .Where(s => s.Status == DeliveryStopStatus.Delivered)
                .Select(s => s.OrderId);

            return query.Where(o =>
                o.Status != OrderStatus.Delivered &&
                o.Status != OrderStatus.Finalized &&
                !deliveredOrderIds.Contains(o.Id));
        }


    }
}