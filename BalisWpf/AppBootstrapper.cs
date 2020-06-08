using System.Windows.Controls.Primitives;

namespace BalisWpf 
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Caliburn.Micro;
    using UtilsLib;

    public class AppBootstrapper : BootstrapperBase {

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
            builder.RegisterModule<AutofacBalisWpf>();

            var iniFile = new IniFile().AssignFile("BalisWpf.ini");
            builder.RegisterInstance(iniFile);

            var logFile = new LogFile(iniFile).AssignFile("BalisWpf.log");
            builder.RegisterInstance(logFile);
            logFile.AppendLine("BalisWpf started");

            _container = builder.Build();
        }
    }
}