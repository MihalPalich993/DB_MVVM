using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DB_MVVM.Data;
using DB_MVVM.Views;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DB_MVVM.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;

        [ObservableProperty] private ClientsViewModel _clientsVM;
        [ObservableProperty] private OrdersViewModel _ordersVM;
        [ObservableProperty] private string _searchText = "";
        [ObservableProperty] private Visibility _adminVisibility = CurrentUser.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        [ObservableProperty] private int _totalClientsCount;
        [ObservableProperty] private int _totalOrdersCount;
        [ObservableProperty] private decimal _allTimeRevenue;

        public MainViewModel()
        {
            _context = new ApplicationDbContext();

            ClientsVM = new ClientsViewModel(_context);
            OrdersVM = new OrdersViewModel(_context);

            // ПОДПИСКИ НА СОБЫТИЯ
            // 1. Когда в ClientsVM меняется клиент -> OrdersVM грузит заказы
            ClientsVM.SelectedClientChangedAction = (client) => OrdersVM.LoadOrders(client);


            // 2. Когда данные меняются в любой из вложенных моделей -> обновляем Dashboard
            ClientsVM.OnDataChanged = UpdateDashboard;
            OrdersVM.OnDataChanged = UpdateDashboard;

            UpdateDashboard();
        }

        public void UpdateDashboard()
        {
            // Было Clients.Count -> Стало ClientsVM.Clients.Count
            TotalClientsCount = ClientsVM.Clients.Count;

            using var db = new ApplicationDbContext();
            TotalOrdersCount = db.Orders.Count();

            // Сумма для SQLite (через ToList)
            AllTimeRevenue = db.Orders
                               .Select(o => o.Amount)
                               .ToList()
                               .Sum();
        }


        partial void OnSearchTextChanged(string value)
        {
            // Было Clients -> Стало ClientsVM.Clients
            var view = CollectionViewSource.GetDefaultView(ClientsVM.Clients);

            view.Filter = (obj) =>
            {
                if (string.IsNullOrWhiteSpace(value)) return true;
                var c = obj as Models.Client;
                return c != null && (c.Surname.Contains(value, System.StringComparison.OrdinalIgnoreCase) ||
                                     c.FirstName.Contains(value, System.StringComparison.OrdinalIgnoreCase));
            };
            view.Refresh();
        }


        [RelayCommand]
        public void ShowLogs() { if (CurrentUser.IsAdmin) new LogWindow().ShowDialog(); }
    }
}