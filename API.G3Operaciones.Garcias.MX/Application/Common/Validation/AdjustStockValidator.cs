using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using FluentValidation;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Validation
{
    public class AdjustStockValidator : AbstractValidator<AdjustStockDto>
    {
        public AdjustStockValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.Reference).NotEmpty().MaximumLength(250);
        }
    }
}
