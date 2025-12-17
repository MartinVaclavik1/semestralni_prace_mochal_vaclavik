using Microsoft.Extensions.DependencyInjection;
using semestralni_prace_mochal_vaclavik.OracleConn;
using semestralni_prace_mochal_vaclavik.Repository;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.ViewModels;
using semestralni_prace_mochal_vaclavik.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace semestralni_prace_mochal_vaclavik
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            
            var services = new ServiceCollection();


            services.AddSingleton<IOracleConnectionFactory>(
            _ => new OracleConnectionFactory("User Id=st72536;" +
                                    "Password=killer12;" +
                                    "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521))" +
                                    "(CONNECT_DATA=(SID=BDAS)));"));

            services.AddTransient<IPolicisteRepository, PolicisteRepository>();
            services.AddTransient<IOkrskyRepository, OkrskyRepository>();
            services.AddTransient<IEvidencePrestupkuRepository, EvidencePrestupkuRepository>();

            services.AddTransient<IPolicisteService, PolicisteService>();
            services.AddTransient<IOkrskyService, OkrskyService>();
            services.AddTransient<IEvidencePrestupkuService, EvidencePrestupkuService>();


            services.AddTransient<PolicisteViewModel>(); 
            services.AddTransient<OkrskyViewModel>();
            services.AddTransient<EvidencePrestupkuViewModel>();
            services.AddTransient<MainViewModel>();

            services.AddSingleton<PolicisteView>();
            services.AddSingleton<OkrskyView>();
            services.AddSingleton<EvidencePrestupkuView>();
            services.AddSingleton<MainWindow>();

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }
    }

}
