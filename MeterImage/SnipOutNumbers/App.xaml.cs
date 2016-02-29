using System.Windows;
using Autofac;

namespace SnipOutNumbers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var builder = new ContainerBuilder();
            builder.RegisterType<ImageHelper>().As<IImageHelper>();
            Container = builder.Build();
        }
    }
}
