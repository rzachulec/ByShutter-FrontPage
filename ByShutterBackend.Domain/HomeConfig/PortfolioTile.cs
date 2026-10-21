using ByShutterBackend.Domain.Portfolio;

namespace ByShutterBackend.Domain.HomeConfig;

public class PortfolioTile
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public string Name { get; set; }
    public string? Description { get; set; }

    public Guid ImageId { get; set; }
    public Photo Photo { get; set; }

    public Guid? HomeProfileId { get; set; }
    public HomeProfile? HomeProfile { get; set; }

    public PortfolioTile(string name, string? description, Guid imageId, Guid? homeProfileId = null)
    {
        Name = name;
        Description = description;
        ImageId = imageId;
        
        HomeProfileId = homeProfileId;
    }
}