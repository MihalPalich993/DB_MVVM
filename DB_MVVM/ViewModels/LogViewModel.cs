using CommunityToolkit.Mvvm.ComponentModel;
using DB_MVVM.Data;
using DB_MVVM.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace DB_MVVM.ViewModels
{
    public partial class LogViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<LogEntry> _logs;

        public LogViewModel()
        {
            using (var db = new ApplicationDbContext())
            {
                // Загружаем логи, сортируем по времени (свежие сверху)
                var list = db.Logs.OrderByDescending(l => l.Timestamp).ToList();
                Logs = new ObservableCollection<LogEntry>(list);
            }
        }
    }
}