using CommunityToolkit.Mvvm.ComponentModel; // Для ObservableObject и ObservableProperty
using CommunityToolkit.Mvvm.Input;        // Для RelayCommand
using DB_MVVM.Data;
using DB_MVVM.Models;
using DB_MVVM.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;     // Для ObservableCollection
using System.Linq;                        // Для работы с LINQ запросами
using System.Threading.Tasks;             // Для асинхронности (async/await)
using System.Windows;                     // Для MessageBox
using System.Windows.Data;

namespace DB_MVVM.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;

        // --- СВОЙСТВА ---

        // Выбранный заказ в нижней таблице
        [ObservableProperty]
        private Order? _selectedOrder;

        // Общая сумма заказов клиента
        [ObservableProperty]
        private decimal _totalOrdersSum;

        // Список клиентов (Верхняя таблица)
        [ObservableProperty]
        private ObservableCollection<Client> _clients;

        // Выбранный клиент
        [ObservableProperty]
        private Client? _selectedClient;

        // Список заказов выбранного клиента (Нижняя таблица)
        [ObservableProperty]
        private ObservableCollection<Order> _clientOrders;

        [ObservableProperty]
        private string _searchText = "";

        // --- КОНСТРУКТОР ---
        public MainViewModel()
        {
            _context = new ApplicationDbContext();

            // Инициализируем коллекции, чтобы не было ошибок NullReference
            Clients = new ObservableCollection<Client>();
            ClientOrders = new ObservableCollection<Order>();

            // Загружаем данные при старте
            LoadData();
        }

        // Просто пробегает по списку и складывает суммы
        private void RecalculateTotal()
        {
            if (ClientOrders == null)
            {
                TotalOrdersSum = 0;
                return;
            }

            // LINQ метод Sum делает всё за нас
            TotalOrdersSum = ClientOrders.Sum(x => x.Amount);
        }

        // --- МЕТОДЫ ЗАГРУЗКИ ---

        public async void LoadData()
        {
            // Убеждаемся, что БД существует
            await _context.Database.EnsureCreatedAsync();

            // Загружаем клиентов из БД
            var list = await _context.Clients.ToListAsync();

            Clients.Clear();
            foreach (var client in list)
            {
                Clients.Add(client);
            }
            // Добавили: Если в поиске что-то было написано при запуске, применим фильтр сразу
            OnSearchTextChanged(SearchText);
        }

        // Магия MVVM Toolkit: этот метод вызывается сам, когда меняется свойство SelectedClient
        partial void OnSelectedClientChanged(Client? value)
        {
            // Как только кликнули на клиента — загружаем его заказы
            LoadOrdersForSelectedClient();
        }

        private void LoadOrdersForSelectedClient()
        {
            // Очищаем старый список заказов
            ClientOrders.Clear();

            if (SelectedClient == null) return;

            // Ищем в БД заказы, где ClientId равен ID выбранного клиента
            // Используем .ToList() (синхронно), так как этот метод вызывается из сеттера свойства
            var orders = _context.Orders
                                 .Where(o => o.ClientId == SelectedClient.Id)
                                 .ToList();

            foreach (var order in orders)
            {
                ClientOrders.Add(order);
            }
            RecalculateTotal();
        }


        // --- КОМАНДЫ (КНОПКИ) ---

        // 1. Добавить Клиента
        [RelayCommand]
        public async Task AddClient()
        {
            var newClient = new Client();

            var window = new ClientWindow
            {
                DataContext = newClient
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync();
                Clients.Add(newClient);
            }
        }

        // 2. Редактировать Клиента
        [RelayCommand]
        public async Task EditClient()
        {
            if (SelectedClient == null) return;

            var window = new ClientWindow
            {
                DataContext = SelectedClient
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
                await _context.SaveChangesAsync();
                // ObservableObject сам уведомит интерфейс об изменениях в свойствах
            }
            else
            {
                // Если нажали "Отмена", нужно откатить изменения в памяти, 
                // иначе в таблице останется измененный текст до перезагрузки программы.
                _context.Entry(SelectedClient).Reload();

                // Трюк для визуального обновления строки в DataGrid:
                var index = Clients.IndexOf(SelectedClient);
                if (index != -1)
                {
                    Clients[index] = SelectedClient;
                }

                // Также перезагрузим заказы на всякий случай
                LoadOrdersForSelectedClient();
            }
        }

        // 3. Удалить Клиента
        [RelayCommand]
        public async Task DeleteClient()
        {
            if (SelectedClient == null) return;

            // Спрашиваем подтверждение (хороший тон)
            var result = MessageBox.Show($"Вы уверены, что хотите удалить клиента {SelectedClient.Surname}?",
                                         "Подтверждение",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.No) return;

            // Важно: При удалении клиента, каскадно удалятся и его заказы (настройки EF Core по умолчанию)
            _context.Clients.Remove(SelectedClient);
            await _context.SaveChangesAsync();

            Clients.Remove(SelectedClient);

            // Очищаем список заказов, так как клиента больше нет
            ClientOrders.Clear();
        }

        // 4. Добавить Заказ (Пока упрощенно)
        [RelayCommand]
        public async Task AddOrder()
        {
            if (SelectedClient == null)
            {
                MessageBox.Show("Сначала выберите клиента в верхней таблице!");
                return;
            }

            // Создаем новый заказ
            var newOrder = new Order
            {
                OrderDate = System.DateTime.Now,
                ClientId = SelectedClient.Id,
                // Начальные значения (пустые, чтобы пользователь ввел сам)
                Products = "",
                Amount = 0
            };

            // Открываем окно
            var window = new OrderWindow
            {
                DataContext = newOrder
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
                // Если нажали сохранить - добавляем в БД
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                // И в таблицу
                ClientOrders.Add(newOrder);
            }
            RecalculateTotal();
        }

        // --- КОМАНДЫ ДЛЯ ЗАКАЗОВ ---

        [RelayCommand]
        public async Task EditOrder()
        {
            if (SelectedOrder == null) return;

            // Открываем то же окно OrderWindow, но передаем текущий заказ
            var window = new OrderWindow
            {
                DataContext = SelectedOrder
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
                // Сохраняем изменения в БД
                await _context.SaveChangesAsync();

                // ВАЖНО: Сумма могла измениться, пересчитываем
                RecalculateTotal();

                // Трюк для обновления строки в таблице (если изменилась дата или товары)
                var index = ClientOrders.IndexOf(SelectedOrder);
                if (index != -1) ClientOrders[index] = SelectedOrder;
            }
            else
            {
                // Откат изменений при отмене
                _context.Entry(SelectedOrder).Reload();

                var index = ClientOrders.IndexOf(SelectedOrder);
                if (index != -1) ClientOrders[index] = SelectedOrder;
            }
        }

        [RelayCommand]
        public async Task DeleteOrder()
        {
            if (SelectedOrder == null) return;

            var result = MessageBox.Show("Удалить этот заказ?", "Подтверждение",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            // Удаляем из БД
            _context.Orders.Remove(SelectedOrder);
            await _context.SaveChangesAsync();

            // Удаляем из списка
            ClientOrders.Remove(SelectedOrder);

            // Пересчитываем сумму (стала меньше)
            RecalculateTotal();
        }

        // Этот метод сработает автоматически при изменении текста в поиске
        partial void OnSearchTextChanged(string value)
        {
            // Получаем "Представление" (View) нашей коллекции клиентов
            var view = CollectionViewSource.GetDefaultView(Clients);

            // Настраиваем фильтр
            view.Filter = (obj) =>
            {
                // Если поиск пустой — показываем всех
                if (string.IsNullOrWhiteSpace(value)) return true;

                // Превращаем объект обратно в Клиента
                var client = obj as Client;
                if (client == null) return false;

                // ПРОВЕРКА: Содержит ли Фамилия введенный текст?
                // StringComparison.OrdinalIgnoreCase означает "не важно, большие или маленькие буквы"
                return client.Surname.Contains(value, System.StringComparison.OrdinalIgnoreCase)
                    || client.FirstName.Contains(value, System.StringComparison.OrdinalIgnoreCase); // Можно искать и по Имени
            };

            // Принудительно обновляем вид
            view.Refresh();
        }
    }
}