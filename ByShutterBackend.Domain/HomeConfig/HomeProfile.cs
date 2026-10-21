using ByShutterBackend.Domain.Common.Exceptions;
using ByShutterBackend.Domain.Portfolio;

namespace ByShutterBackend.Domain.HomeConfig;

public class HomeProfile
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public string HeroMessage { get; set; }

    public Guid HeroImageId { get; set; }
    public Photo HeroImage { get; set; }

    public string Greetings { get; set; }
    public string Quote { get; set; }
    public string AboutMe { get; set; }

    public Guid ProfileImageId { get; set; }
    public Photo ProfileImage { get; set; }

    public ICollection<PortfolioTile> PortfolioTiles { get; init; } = [];

    public HomeProfile(string heroMessage, string greetings, string quote, string aboutMe, Guid heroImageId,
        Guid profileImageId)
    {
        HeroMessage = heroMessage;
        Greetings = greetings;
        Quote = quote;
        AboutMe = aboutMe;
        HeroImageId = heroImageId;
        ProfileImageId = profileImageId;
    }

    /// <summary>
    /// Add portfolio tile to the home profile.
    /// </summary>
    /// <param name="portfolioTile">Portfolio tile to append</param>
    /// <exception cref="DuplicationError">Thrown when the home profile already contains tile</exception>
    /// <exception cref="OverflowException">Thrown when the home profile has 4 or more tiles</exception>
    public void AddPortfolioTile(PortfolioTile portfolioTile)
    {
        if (PortfolioTiles.Contains(portfolioTile))
        {
            throw new DuplicationException(portfolioTile, nameof(PortfolioTiles));
        }

        if (PortfolioTiles.Count >= 4)
        {
            throw new OverflowException();
        }
        
        PortfolioTiles.Add(portfolioTile);
    }

    public void RemovePortfolioTile(Guid portfolioTileId)
    {
        var portfolioTile = PortfolioTiles.FirstOrDefault(tile => tile.Id == portfolioTileId);

        if (portfolioTile is not null)
        {
            PortfolioTiles.Remove(portfolioTile);
        }
    }
}