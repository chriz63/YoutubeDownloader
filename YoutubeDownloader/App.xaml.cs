using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using YoutubeDownloader.Helper;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider= services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IDataRetriever, DataRetriever>();
            services.AddSingleton<IConfigurationChanger, ConfigurationChanger>();
            services.AddSingleton<MainWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
