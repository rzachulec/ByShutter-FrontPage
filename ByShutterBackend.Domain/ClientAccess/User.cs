namespace ByShutterBackend.Domain.ClientAccess;

public class User
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string PasswordSalt { get; private set; }

    private User()
    {
        Username = string.Empty;
        PasswordHash = string.Empty;
        PasswordSalt = string.Empty;
    }

    public User(string username, string passwordHash, string passwordSalt)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
}
