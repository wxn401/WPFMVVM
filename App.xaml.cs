using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfCore.View;
using WpfCore.ViewModel;

namespace WpfCore
{
    /// <summary>
    /// 原文：https://lebang2020.cn/details/211106ffdi3wri.html
    /// </summary>
    public partial class App : Application
    {
        public IConfiguration Configuration { get; private set; }

        public VktServiceProviderFactory ServiceProviderFactory { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProviderFactory = new VktServiceProviderFactory();
            var host = CreateHostBuilder().Build();
            var serviceProvider = host.Services;

            var mainviewmodel = serviceProvider.GetRequiredService<MainViewModel>();
            var mainview = new MainView(); 
            mainview.DataContext = mainviewmodel;
            mainview.Show();
        }

        private IHostBuilder CreateHostBuilder() =>
           Host.CreateDefaultBuilder().UseServiceProviderFactory(ServiceProviderFactory)
               .ConfigureHostConfiguration(config =>
               {
                   Configuration = config.Build();
               }).ConfigureServices(services =>
               {
                   services.AddTransient(typeof(MainViewModel));
                   services.AddTransient(typeof(NetUnitViewModel));
               });

    }
}
