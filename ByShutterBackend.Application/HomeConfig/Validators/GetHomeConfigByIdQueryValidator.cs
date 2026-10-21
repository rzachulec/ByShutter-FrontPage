using ByShutterBackend.Application.HomeConfig.Queries;
using FluentValidation;

namespace ByShutterBackend.Application.HomeConfig.Validators;

public class GetHomeConfigByIdQueryValidator : AbstractValidator<GetHomeConfigByIdQuery>
{
    public GetHomeConfigByIdQueryValidator()
    {
        RuleFor(query => query.HomeConfigId)
            .NotEmpty()
            .NotNull();
    }    
}