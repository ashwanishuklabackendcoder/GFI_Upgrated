namespace GFI_Upgrated.SharedDto.Common;

public class ApiException : Exception
{
    public ApiException() : base() { }

    public ApiException(string message) : base(message) { }

    public ApiException(string message, Exception innerException) : base(message, innerException) { }
}
