using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_MVVM.Models
{
    // Наследуем от ObservableValidator для проверки ошибок (пустая сумма, пустые товары)
    public partial class Order : ObservableValidator
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Поля для MVVM
        private string _products = string.Empty;
        private decimal _amount;
        private string? _deliveryAddress; // Может быть null

        [Required(ErrorMessage = "Укажите состав заказа")]
        public string Products
        {
            get => _products;
            set => SetProperty(ref _products, value, true);
        }

        [Range(0.01, 1000000, ErrorMessage = "Сумма должна быть больше 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value, true);
        }

        // Новое поле!
        public string? DeliveryAddress
        {
            get => _deliveryAddress;
            set => SetProperty(ref _deliveryAddress, value, true);
        }

        // Внешний ключ
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        // Метод для вызова валидации извне (как мы делали в Client)
        public void Validate() => ValidateAllProperties();
    }
}