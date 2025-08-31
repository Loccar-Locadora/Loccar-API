namespace LoccarDomain;

public class BaseReturn<T>
{
    public string Code { get; set; }   // melhor usar propriedades
    public string Message { get; set; }
    public T Data { get; set; }
}