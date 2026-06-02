using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace API.G3Operaciones.Garcias.MX.Application.Delivery.Services
{
    public class PdfTicketService : IPdfTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PdfTicketService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Genera y guarda el ticket PDF de una entrega.
        public async Task<string> GenerateDeliveryTicketAsync(Guid deliveryStopId)
        {
            var stop = await _context.DeliveryStops
                .Include(x => x.DeliveryPlan)
                .FirstOrDefaultAsync(x => x.Id == deliveryStopId)
                ?? throw new KeyNotFoundException("Stop no encontrado");

            var order = await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == stop.OrderId)
                ?? throw new KeyNotFoundException("Orden no encontrada");

            var client = await _context.Clients
                .FirstOrDefaultAsync(x => x.Id == stop.ClientId)
                ?? throw new KeyNotFoundException("Cliente no encontrado");

            var chainName = client.ChainId.HasValue
                ? await _context.Chains
                    .Where(x => x.Id == client.ChainId.Value)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync()
                : null;

            // 🔥 Productos SOLO para nombre (precio ya viene en OrderItem)
            var productIds = order.Items.Select(x => x.ProductId).ToList();

            var products = await _context.Products
                .Where(x => productIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            // 🔥 Usuario (NO FullName inventado)
            var driverName = await _context.Users
     .Where(x => x.Id == stop.DeliveryPlan.DeliveryUserId)
     .Select(x => x.FirstName + " " + x.LastName)
     .FirstOrDefaultAsync() ?? "N/D";

            var dto = new DeliveryTicketDto
            {
                CompanyName = "G3 Operaciones Garcia's MX",
                BranchName = client.Name,
                ChainName = chainName,
                Address = client.Address,
                DriverName = driverName,
                DeliveredAt = DateTime.UtcNow,
                Items = order.Items.Select(x => new DeliveryTicketItemDto
                {
                    ProductName = products.ContainsKey(x.ProductId)
                        ? products[x.ProductId]
                        : "Producto",
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice // 🔥 VIENE DE LA ORDEN
                }).ToList(),
                Subtotal = order.Items.Sum(x => x.Quantity * x.UnitPrice),
                Total = order.TotalAmount
            };

            return await GeneratePdf(dto, deliveryStopId);
        }

        private static void ConfigurePage(PageDescriptor page, DeliveryTicketDto dto, string copyType)
        {
            page.Size(PageSizes.A6);
            page.Margin(15);

            page.DefaultTextStyle(x => x.FontSize(9));

            page.Content().Column(col =>
            {
                col.Spacing(6);

                // 🔷 HEADER
                col.Item().AlignCenter().Column(header =>
                {
                    header.Item().Text(dto.CompanyName)
                        .Bold().FontSize(14);

                    header.Item().Text("TICKET DE ENTREGA")
                        .FontSize(10);

                    header.Item().Text(copyType)
                        .FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                });

                col.Item().LineHorizontal(1);

                // 🔷 INFORMACIÓN GENERAL
                col.Item().Column(info =>
                {
                    info.Spacing(2);

                    info.Item().Text($"Sucursal: {dto.BranchName}");

                    if (!string.IsNullOrWhiteSpace(dto.ChainName))
                        info.Item().Text($"Cadena: {dto.ChainName}");

                    info.Item().Text($"Dirección: {dto.Address}");
                    info.Item().Text($"Repartidor: {dto.DriverName}");
                    info.Item().Text($"Fecha: {dto.DeliveredAt:dd/MM/yyyy HH:mm}");
                });

                col.Item().LineHorizontal(1);

                // 🔷 TABLA DE PRODUCTOS
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                    });

                    // HEADER
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Producto").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Cant").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Total").Bold();
                    });

                    // BODY
                    foreach (var item in dto.Items)
                    {
                        table.Cell().Padding(2).Text(item.ProductName);
                        table.Cell().Padding(2).AlignRight().Text(item.Quantity.ToString());
                        table.Cell().Padding(2).AlignRight().Text($"${item.Total:N2}");
                    }
                });

                col.Item().LineHorizontal(1);

                // 🔷 TOTALES
                col.Item().AlignRight().Column(totals =>
                {
                    totals.Item().Text($"Subtotal: ${dto.Subtotal:N2}");
                    totals.Item().Text($"Total: ${dto.Total:N2}")
                        .Bold().FontSize(11);
                });

                col.Item().LineHorizontal(1);

                // 🔷 FIRMAS (PRO REAL)
                col.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(colFirma =>
                    {
                        colFirma.Item().Height(40).BorderBottom(1);
                        colFirma.Item().AlignCenter().Text("Firma Cliente").FontSize(8);
                    });

                    row.ConstantItem(20);

                    row.RelativeItem().Column(colFirma =>
                    {
                        colFirma.Item().Height(40).BorderBottom(1);
                        colFirma.Item().AlignCenter().Text("Firma Repartidor").FontSize(8);
                    });
                });

                col.Item().PaddingTop(8)
                    .AlignCenter()
                    .Text("Gracias por su preferencia")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        private async Task<string> GeneratePdf(DeliveryTicketDto dto, Guid stopId)
        {
            var folder = "delivery-tickets";
            var root = _environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var path = Path.Combine(root, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = $"{stopId:N}.pdf";
            var fullPath = Path.Combine(path, fileName);
            var relative = $"/{folder}/{fileName}";

            if (File.Exists(fullPath))
                return relative;

            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                // 🔥 COPIA 1
                container.Page(page =>
                {
                    ConfigurePage(page, dto, "COPIA SUCURSAL");
                });

                // 🔥 COPIA 2
                container.Page(page =>
                {
                    ConfigurePage(page, dto, "COPIA REPARTIDOR");
                });

            }).GeneratePdf(fullPath);

            return relative;
        }
    }
    }