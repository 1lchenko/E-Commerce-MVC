namespace ECommerce.Exceptions;

public class EditOrderException : Exception
{
    public EditOrderException(string message, Exception e ) : base(message,e)
    {
        
    }
}