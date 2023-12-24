namespace ECommerce.Exceptions;

public class AddImageException : Exception
{
    public AddImageException(string message, Exception e ) : base(message,e)
    {
    }
}