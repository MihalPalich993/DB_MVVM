using CommunityToolkit.Mvvm.ComponentModel; // Библиотека MVVM
using System.ComponentModel.DataAnnotations; // Библиотека Атрибутов

namespace DB_MVVM.Models
{
    // 1. Наследуемся от ObservableValidator (он умеет проверять ошибки)
    public partial class Client : ObservableValidator
    {
        public int Id { get; set; }

        // Поля для хранения значений (backing fields)
        private string _surname = string.Empty;
        private string _firstName = string.Empty;
        private string _phone = string.Empty;
        private string _email = string.Empty;
        private string _address = string.Empty;
        private string _status = "Обычный";

        // 2. СВОЙСТВА С ВАЛИДАЦИЕЙ

        [Required(ErrorMessage = "Фамилия обязательна")]
        [MinLength(2, ErrorMessage = "Минимум 2 буквы")]
        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value, true); // true включает валидацию
        }

        [Required(ErrorMessage = "Имя обязательно")]
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value, true);
        }

        [Required(ErrorMessage = "Телефон обязателен")]
        [Phone(ErrorMessage = "Некорректный формат")]
        [MinLength(11, ErrorMessage = "Должно быть 11 цифр")]
        [MaxLength(12, ErrorMessage = "Максимум 12 цифр")]
        // Можно использовать регулярное выражение для только цифр:
        // [RegularExpression(@"^\d+$", ErrorMessage = "Только цифры")]
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value, true);
        }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Нет символа @ или неверный формат")]
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value, true);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value, true);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, true);
        }

        // Вспомогательное свойство (не в базу)
        public string FullName => $"{Surname} {FirstName}";

        public void Validate()
        {
            ValidateAllProperties();
        }

        // Навигационное свойство: Список заказов этого клиента
        // virtual нужен для Entity Framework
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}