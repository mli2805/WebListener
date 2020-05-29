using Autofac;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public sealed class AutofacBalisWpf : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
            builder.RegisterType<LogFile>().As<IMyLog>().InstancePerLifetimeScope();
            builder.RegisterType<ShellViewModel>().As<IShell>();

        }
    }
}
