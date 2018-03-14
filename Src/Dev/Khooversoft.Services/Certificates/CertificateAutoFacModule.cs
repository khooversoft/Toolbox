using Autofac;

namespace Khooversoft.Services
{
    public class CertificateAutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CertificateActor>().As<ICertificateActor>();
            builder.RegisterType<CertificateActorRepository>().As<ICertificateRepository>().SingleInstance();
        }
    }

    public static class CertificateAutoFacModuleExtension
    {
        public static ContainerBuilder AddCertificateModule(this ContainerBuilder self)
        {
            self.RegisterModule(new CertificateAutoFacModule());
            return self;
        }
    }
}
