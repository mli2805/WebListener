using Autofac;
using Caliburn.Micro;
using UtilsLib;

namespace Balis2021
{
    public class AutofacBalis2021 : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<LogFile>().As<IMyLog>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IShell>();

            builder.RegisterType<AllKomBanksViewModel>().SingleInstance();

        }

    }
}
