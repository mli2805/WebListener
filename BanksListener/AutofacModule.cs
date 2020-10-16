using Autofac;
using BalisStandard;
using UtilsLib;

namespace BanksListener
{
    public sealed class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var iniFile = new IniFile();
            iniFile.AssignFile("baliWebApi.ini");
            builder.RegisterInstance(iniFile);

            var logFile = new LogFile(iniFile);
            logFile.AssignFile("baliWebApi.log");
            builder.RegisterInstance<IMyLog>(logFile);

            builder.RegisterType<BalisSignalRClient>();
        }
    }
}