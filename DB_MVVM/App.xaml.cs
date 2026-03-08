using System.Globalization;
using System.Windows;
using System.Windows.Markup; // Нужно для XmlLanguage

namespace DB_MVVM
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 1. Устанавливаем культуру для кода (вычисления, даты в C#)
            var cultureInfo = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // 2. ГЛАВНЫЙ МОМЕНТ: Устанавливаем культуру для XAML (интерфейса)
            // Это заставит все Binding-и с StringFormat=C видеть рубли
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            base.OnStartup(e);
        }
    }
}