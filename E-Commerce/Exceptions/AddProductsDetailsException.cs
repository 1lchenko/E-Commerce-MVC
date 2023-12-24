namespace ECommerce.Exceptions;

public class AddProductsDetailsException : Exception
{
    public AddProductsDetailsException(string message, Exception e ) : base(message,e)
    {
    }
}