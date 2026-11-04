using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.FileStorage.Commands;

public class OptimizeImageCommandHandler : IRequestHandler<OptimizeImageCommand, byte[]>
{
    private readonly IImageOptimizer _imageOptimizer;

    public OptimizeImageCommandHandler(IImageOptimizer imageOptimizer)
    {
        _imageOptimizer = imageOptimizer;
    }

    public Task<byte[]> Handle(OptimizeImageCommand request, CancellationToken cancellationToken)
    {
        return _imageOptimizer.CreateWebRenditionAsync(request.Source, cancellationToken);
    }
}
