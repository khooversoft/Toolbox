using Autofac;
using FluentAssertions;
using Khooversoft.Actor;
using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khoover.Services.Test.Certificate
{
    [Trait("Category", "Certificate Actor")]
    public class CertificateActorTests
    {
        private static IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public async Task CertificateSetGetTest()
        {
            const string testData = "Test data, test, data";

            var builder = new ContainerBuilder();
            builder.RegisterModule(new CertificateAutoFacModule());
            ILifetimeScope container = builder.Build();

            IActorManager manager = new ActorManagerBuilder()
                .Set(container)
                .Build();

            using (ILifetimeScope scopeContainer = container.BeginLifetimeScope(x => x.RegisterInstance(manager)))
            {
                var context = _workContext.ToBuilder()
                    .SetContainer(scopeContainer)
                    .Build();

                var key = new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true);

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
