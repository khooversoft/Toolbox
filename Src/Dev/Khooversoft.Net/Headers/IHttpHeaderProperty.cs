namespace Khooversoft.Net
{
    public interface IHttpHeaderProperty
    {
        string Key { get; }
        string FormatValueForHttp();
    }
}
