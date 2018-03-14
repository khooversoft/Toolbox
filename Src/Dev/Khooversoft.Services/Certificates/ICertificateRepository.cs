using Khooversoft.Security;
using Khooversoft.Toolbox;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface ICertificateRepository
    {
        Task<X509Certificate2> GetCertificate(IWorkContext context, LocalCertificateKey certificateKey, bool throwOnNotFound);

        Task<byte[]> Encrypt(IWorkContext context, LocalCertificateKey certificateKey, byte[] data);

        Task<byte[]> Decrypt(IWorkContext context, LocalCertificateKey certificateKey, byte[] data);
    }
}
