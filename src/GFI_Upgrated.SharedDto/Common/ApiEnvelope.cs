namespace GFI_Upgrated.SharedDto.Common;

public sealed class ApiEnvelope<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

