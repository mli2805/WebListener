using Autofac;
using BalisStandard;
using UtilsLib;

namespace BanksListener
{
    public static class AutofacBali
    {
        public static ContainerBuilder WithProduction(this ContainerBuilder builder)
        {
            var iniFile = new IniFile().AssignFile("baliSelf.ini");
            builder.RegisterInstance(iniFile);

            var logFile = new LogFile(iniFile, 2000).AssignFile("baliSelf.log");
            builder.RegisterInstance(logFile);

            builder.RegisterType<BalisSignalRClient>();

            return builder;
        }
    }
}
