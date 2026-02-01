using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DB_MVVM.Data;
using DB_MVVM.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DB_MVVM.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApplicationDbContext _context;

        [ObservableProperty]
        private ObservableCollection<Client> _clients;

        [ObservableProperty]
        private Client? _selectedClient;

        public MainViewModel()
        {
            _context = new ApplicationDbContext();
            Clients = new ObservableCollection<Client>();
            LoadData();
        }

        public async void LoadData()
        {
            await _context.Database.EnsureCreatedAsync();
            var list = await _context.Clients.ToListAsync();
            Clients.Clear();
            foreach (var client in list)
            {
                Clients.Add(client);
            }
        }

        [RelayCommand]
        public async Task AddTestClient()
        {
            var newClient = new Client
            {
                FirstName = "Михаил",
                Surname = "Ухлинов",
                Phone = "89629405650",
                Email = "uhlinovmihail@gmail.com",
                Address = "Москва",
                Status = "VIP"
            };

            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();
            Clients.Add(newClient);
        }

        [RelayCommand]
        public async Task DeleteClient()
        {
            if (SelectedClient == null) return;

            _context.Clients.Remove(SelectedClient);
            await _context.SaveChangesAsync();
            Clients.Remove(SelectedClient);
        }
    }
}