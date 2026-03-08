using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DB_MVVM.Data;
using DB_MVVM.Models;
using DB_MVVM.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace DB_MVVM.ViewModels
{
    public partial class ClientsViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;
        public Action? OnDataChanged;

        // ПЕРЕИМЕНОВАЛИ: Добавили слово Action в конец
        public Action<Client?>? SelectedClientChangedAction;

        [ObservableProperty]
        private Client? _selectedClient;

        // Поле с маленькой буквы и нижним подчеркиванием
        [ObservableProperty]
        private ObservableCollection<Client> _clients = new();

        // Теперь конфликта нет: метод называется OnSelectedClientChanged, 
        // а событие — SelectedClientChangedAction
        partial void OnSelectedClientChanged(Client? value)
        {
            SelectedClientChangedAction?.Invoke(value);
        }


        public ClientsViewModel(ApplicationDbContext context)
        {
            _context = context;
            LoadClients();
        }

        public async void LoadClients()
        {
            await _context.Database.EnsureCreatedAsync();
            var list = await _context.Clients.ToListAsync();
            Clients.Clear();
            foreach (var c in list) Clients.Add(c);
            OnDataChanged?.Invoke();
        }

        [RelayCommand]
        public async Task AddClient()
        {
            var newClient = new Client();
            if (new ClientWindow { DataContext = newClient }.ShowDialog() == true)
            {
                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync();
                Clients.Add(newClient);
                OnDataChanged?.Invoke();
            }
        }

        [RelayCommand]
        public async Task EditClient()
        {
            if (SelectedClient == null) return;
            if (new ClientWindow { DataContext = SelectedClient }.ShowDialog() == true)
            {
                await _context.SaveChangesAsync();
                OnDataChanged?.Invoke();
            }
            else
            {
                _context.Entry(SelectedClient).Reload();
                var index = Clients.IndexOf(SelectedClient);
                if (index != -1) Clients[index] = SelectedClient;
            }
        }

        [RelayCommand]
        public async Task DeleteClient()
        {
            if (SelectedClient == null) return;
            if (MessageBox.Show($"Удалить {SelectedClient.Surname}?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Clients.Remove(SelectedClient);
                await _context.SaveChangesAsync();
                Clients.Remove(SelectedClient);
                OnDataChanged?.Invoke();
            }
        }
    }
}