using ByShutterBackend.Application.Portfolio.Commands;
using FluentValidation;

namespace ByShutterBackend.Application.Portfolio.Validators;

public class CreateAlbumCommandValidator : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumCommandValidator()
    {
        RuleFor(album => album.AlbumName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(32);
        
        RuleFor(album => album.Description)
            .MaximumLength(256);
    }
}