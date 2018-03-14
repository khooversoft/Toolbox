using Khooversoft.Actor;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    /// <summary>
    /// Actor for managing a certificate
    /// Actor key = certificate's thumbprint
    /// </summary>
    public class CertificateActor : ActorBase, ICertificateActor
    {
        private LocalCertificate _localCertificate;

        public CertificateActor(ActorKey actorKey, IActorManager actorManager)
            : base(actorKey, actorManager)
        {
        }

        protected override Task OnActivate(IWorkContext context)
        {
            _localCertificate = new LocalCertificate(new LocalCertificateKey(ActorKey.VectorKey));

            return base.OnActivate(context);
        }

        public Task<byte[]> Decrypt(IWorkContext context, byte[] data)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(data), data);

            byte[] values = _localCertificate.GetCertificate(context).Decrypt(data);
            return Task.FromResult(values);
        }

        public Task<byte[]> Encrypt(IWorkContext context, byte[] data)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(data), data);

            byte[] values = _localCertificate.GetCertificate(context).Encrypt(data);
            return Task.FromResult(values);
        }

        public Task<X509Certificate2> GetCertificate(IWorkContext context, bool throwOnNotFound)
        {
            Verify.IsNotNull(nameof(context), context);

            return Task.FromResult(_localCertificate.GetCertificate(context, throwOnNotFound: throwOnNotFound));
        }
    }
}

