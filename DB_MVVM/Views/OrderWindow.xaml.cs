using DB_MVVM.Models;
using System.Linq;
using System.Windows;

namespace DB_MVVM.Views
{
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            var order = (Order)DataContext;

            // 1. Валидация (проверка заполнения суммы и товаров)
            order.Validate();
            if (order.HasErrors)
            {
                var errors = string.Join("\n", order.GetErrors().Select(x => x.ErrorMessage));
                MessageBox.Show($"Ошибка:\n{errors}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. БИЗНЕС-ЛОГИКА: "Самовывоз"
            // Если поле пустое или состоит из пробелов -> пишем "Самовывоз"
            if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                order.DeliveryAddress = "Самовывоз";
            }

            this.DialogResult = true;
        }
    }
}