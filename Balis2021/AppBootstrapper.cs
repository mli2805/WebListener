using Autofac;
using UtilsLib;

namespace Balis2021 {
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;

    public class AppBootstrapper : BootstrapperBase 
    {
        private ILifetimeScope _container;

        public AppBootstrapper() {
            Initialize();
        }

        protected override void Configure() {
         
        }

        protected override object GetInstance(Type service, string key) {
            return string.IsNullOrWhiteSpace(key) ?
                _container.Resolve(service) :
                _container.ResolveNamed(key, service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance) {
            _container.InjectProperties(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e) {
            SomeInitialActions();
            DisplayRootViewFor<IShell>();
        }

        private void SomeInitialActions()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacBalis2021>();

            var iniFile = new IniFile().AssignFile("Balis2021.ini");
            builder.RegisterInstance(iniFile);

            var logFile = new LogFile(iniFile).AssignFile("Balis2021.log");
            builder.RegisterInstance(logFile);
            logFile.AppendLine("BalisWpf started");

            _container = builder.Build();
        }
    }
}