using ByShutterBackend.Application.HomeConfig.Commands;
using FluentValidation;

namespace ByShutterBackend.Application.HomeConfig.Validators;

public class CreateHomeProfileCommandValidator : AbstractValidator<CreateHomeProfileCommand>
{
    public CreateHomeProfileCommandValidator()
    {
        RuleFor(command => command.HeroMessage)
            .NotEmpty()
            .NotNull();
        
        RuleFor(command => command.Greetings)
            .NotEmpty()
            .NotNull();
        
        RuleFor(command => command.Quote)
            .NotEmpty()
            .NotNull();
        
        RuleFor(command => command.AboutMe)
            .NotEmpty()
            .NotNull();
        
    }
}