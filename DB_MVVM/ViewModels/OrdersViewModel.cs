using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DB_MVVM.Data;
using DB_MVVM.Models;
using DB_MVVM.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DB_MVVM.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;
        public Action? OnDataChanged; // Для обновления Dashboard

        [ObservableProperty] private ObservableCollection<Order> _clientOrders = new();
        [ObservableProperty] private Order? _selectedOrder;
        [ObservableProperty] private decimal _totalOrdersSum;

        public OrdersViewModel(ApplicationDbContext context) => _context = context;

        public void LoadOrders(Client? client)
        {
            ClientOrders.Clear();
            if (client != null)
            {
                var orders = _context.Orders.Where(o => o.ClientId == client.Id).ToList();
                foreach (var o in orders) ClientOrders.Add(o);
            }
            RecalculateTotal();
        }

        private void RecalculateTotal() => TotalOrdersSum = ClientOrders.Sum(x => x.Amount);

        [RelayCommand]
        public async Task AddOrder(Client? currentClient)
        {
            if (currentClient == null) return;
            var newOrder = new Order { OrderDate = DateTime.Now, ClientId = currentClient.Id };
            if (new OrderWindow { DataContext = newOrder }.ShowDialog() == true)
            {
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();
                ClientOrders.Add(newOrder);
                RecalculateTotal();
                OnDataChanged?.Invoke();
            }
        }

        [RelayCommand]
        public async Task EditOrder()
        {
            if (SelectedOrder == null) return;
            if (new OrderWindow { DataContext = SelectedOrder }.ShowDialog() == true)
            {
                await _context.SaveChangesAsync();
                RecalculateTotal();
                OnDataChanged?.Invoke();
            }
            else
            {
                _context.Entry(SelectedOrder).Reload();
                var index = ClientOrders.IndexOf(SelectedOrder);
                if (index != -1) ClientOrders[index] = SelectedOrder;
            }
        }

        [RelayCommand]
        public async Task DeleteOrder()
        {
            if (SelectedOrder == null) return;
            if (MessageBox.Show("Удалить заказ?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Orders.Remove(SelectedOrder);
                await _context.SaveChangesAsync();
                ClientOrders.Remove(SelectedOrder);
                RecalculateTotal();
                OnDataChanged?.Invoke();
            }
        }
    }
}