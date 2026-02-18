using DB_MVVM.Models; // Добавь это, чтобы видеть класс Client
using System.Windows;
using System.Linq; // Для проверки ошибок

namespace DB_MVVM.Views
{
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            var client = (Client)DataContext;

            // 1. Вызываем наш публичный метод, который запустит проверку
            client.Validate();

            // 2. Проверяем ошибки
            if (client.HasErrors)
            {
                // GetErrors() возвращает список ошибок валидации
                var errors = string.Join("\n", client.GetErrors().Select(x => x.ErrorMessage));

                MessageBox.Show($"Ошибки:\n{errors}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Не закрываем окно!
            }

            this.DialogResult = true;
        }
    }
}