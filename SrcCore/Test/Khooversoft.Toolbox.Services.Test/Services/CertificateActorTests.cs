using Autofac;
using FluentAssertions;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Services;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Services.Test.Services
{
    [Trait("Category", "Certificate Actor")]
    public class CertificateActorTests
    {
        private static IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public async Task CertificateSetGetTest()
        {
            const string testData = "Test data, test, data";
            IWorkContext context = _workContext.WithMethodName();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new CertificateAutoFacModule());
            ILifetimeScope container = builder.Build();

            IActorManager manager = new ActorConfigurationBuilder()
                .AddCertificateModule(container)
                .Build()
                .ToActorManager();

            using (ILifetimeScope scopeContainer = container.BeginLifetimeScope())
            using (manager)
            {
                var key = new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, Constants.JwtVaultTestCertificateThumbprint, true);

                ICertificateActor actor = await manager.CreateProxyAsync<ICertificateActor>(context, key.CreateActorKey());
                byte[] rawBytes = Encoding.UTF8.GetBytes(testData);
                byte[] encryptedBytes = await actor.Encrypt(context, rawBytes);

                byte[] unencrypted = await actor.Decrypt(context, encryptedBytes);
                string result = Encoding.UTF8.GetString(unencrypted);

                result.Should().Be(testData);
            }

            await Verify.AssertExceptionAsync<ArgumentException>(async () => await manager.DeactivateAllAsync(_workContext));
        }
    }
}
