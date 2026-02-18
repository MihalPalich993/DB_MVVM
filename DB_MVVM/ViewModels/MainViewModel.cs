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

namespace DB_MVVM.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;

        // --- СВОЙСТВА ---

        // Список клиентов (Верхняя таблица)
        [ObservableProperty]
        private ObservableCollection<Client> _clients;

        // Выбранный клиент
        [ObservableProperty]
        private Client? _selectedClient;

        // Список заказов выбранного клиента (Нижняя таблица)
        [ObservableProperty]
        private ObservableCollection<Order> _clientOrders;


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

            var newOrder = new Order
            {
                OrderDate = System.DateTime.Now,
                Products = "Новый заказ (Стул)", // Хардкод для теста
                Amount = 1500,
                ClientId = SelectedClient.Id
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            ClientOrders.Add(newOrder);
        }
    }
}