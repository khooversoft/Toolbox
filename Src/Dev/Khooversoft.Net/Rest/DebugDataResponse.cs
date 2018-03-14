namespace Khooversoft.Net
{
    public class DebugDataResponse<T>
    {
        public T Value { get; set; }

        public DebugEventContractV1 DebugEvents { get; set; }
    }
}
