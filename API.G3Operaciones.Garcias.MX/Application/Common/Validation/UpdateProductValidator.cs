using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using FluentValidation;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Validation
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Presentation)
                .MaximumLength(150);

            RuleFor(x => x.Image)
                .Must(BeValidImage)
                .When(x => x.Image != null)
                .WithMessage("Formato de imagen no permitido.");
        }

        private bool BeValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(extension)
                   && file.Length <= 5 * 1024 * 1024; // 5MB
        }
    }
}