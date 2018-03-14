using Khooversoft.Toolbox;

namespace Khooversoft.Net
{
    public static class RestClientExtensions
    {
        public static RestClient SetCv(this RestClient self, CorrelationVector cv = null)
        {
            Verify.IsNotNull(nameof(self), self);

            cv = cv ?? new CorrelationVector();

            self.Properties.TryAdd(() => new CvHeader(cv));
            return self;
        }
    }
}
