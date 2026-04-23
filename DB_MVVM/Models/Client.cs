using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations; 
namespace DB_MVVM.Models
{
    public partial class Client : ObservableValidator
    {
        public int Id { get; set; }

        private string _surname = string.Empty;
        private string _firstName = string.Empty;
        private string _phone = string.Empty;
        private string _email = string.Empty;
        private string _address = string.Empty;
        private string _status = "Обычный";

        [Required(ErrorMessage = "Фамилия обязательна")]
        [MinLength(2, ErrorMessage = "Минимум 2 буквы")]
        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value, true);
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
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value, true);
        }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Нет символа @ или неверный формат")]
        [RegularExpression(@"^[a-zA-Z0-9@._-]+$", ErrorMessage = "Почта может содержать только латинские буквы и спецсимволы")]
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

        public string FullName => $"{Surname} {FirstName}";

        public void Validate()
        {
            ValidateAllProperties();
        }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
