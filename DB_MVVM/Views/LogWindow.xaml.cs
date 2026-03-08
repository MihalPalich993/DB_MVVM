using System.Windows;
using DB_MVVM.ViewModels; // Подключаем пространство имен наших ViewModel

namespace DB_MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();

            // Создаем экземпляр LogViewModel и назначаем его в DataContext.
            // Именно благодаря этой строке привязки (Binding) в XAML-файле 
            // "поймут", откуда брать список логов.
            this.DataContext = new LogViewModel();
        }

        // В чистое MVVM-окно обычно больше ничего не пишут. 
        // Вся логика загрузки данных находится в LogViewModel.
    }
}