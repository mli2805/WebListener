using Autofac;
using Caliburn.Micro;
using UtilsLib;

namespace BalisWpf
{
    public sealed class AutofacBalisWpf : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<LogFile>().As<IMyLog>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IShell>();

            builder.RegisterType<ShellVm>();
            builder.RegisterType<BelStockViewModel>();
            builder.RegisterType<BelStockArchiveViewModel>();
            builder.RegisterType<MonthlyChartViewModel>();
        }
    }
}
