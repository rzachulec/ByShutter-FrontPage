namespace ByShutterBackend.Domain.Common.Exceptions;

public class DuplicationException : Exception
{
    public DuplicationException(object item, string collectionName) : base(
        $"{item} already exists in {collectionName}.")
    {
        
    }
}