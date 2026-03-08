using System.Linq;
using System.Windows;
using DB_MVVM.Data;
using DB_MVVM.Models;

namespace DB_MVVM.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new ApplicationDbContext())
            {
                // Убеждаемся, что база и таблицы созданы
                db.Database.EnsureCreated();

                // Если пользователей вообще нет (первый запуск), создаем стандартных
                if (!db.Users.Any())
                {
                    db.Users.Add(new User { Username = "admin", Password = "123", Role = "Admin" });
                    db.Users.Add(new User { Username = "manager", Password = "123", Role = "Manager" });
                    db.SaveChanges();
                }

                // Загружаем список пользователей в ComboBox
                cbLogin.ItemsSource = db.Users.ToList();
                cbLogin.SelectedIndex = 0; // Выбираем первого по умолчанию
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текст из ComboBox (т.к. IsEditable=True, пользователь может и вводить текст)
            string typedLogin = cbLogin.Text;
            string password = pbPassword.Password;

            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == typedLogin && u.Password == password);

                if (user != null)
                {
                    // Сохраняем данные вошедшего пользователя
                    CurrentUser.Name = user.Username;
                    CurrentUser.Role = user.Role;

                    // Открываем главное окно
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}