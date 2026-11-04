using ByShutterBackend.Application.Portfolio.Commands;
using FluentValidation;

namespace ByShutterBackend.Application.Portfolio.Validators;

public class UpdateAlbumCommandValidator : AbstractValidator<UpdateAlbumCommand>
{
    public UpdateAlbumCommandValidator()
    {
        RuleFor(album => album.Name)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(album => album.Description)
            .MaximumLength(256);
    }
}
