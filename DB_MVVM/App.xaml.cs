using System.Globalization;
using System.Threading;
using System.Windows;

namespace DB_MVVM
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Создаем русскую культуру
            var culture = new CultureInfo("ru-RU");

            // Применяем её ко всем потокам приложения
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Стандартный запуск
            base.OnStartup(e);
        }
    }
}