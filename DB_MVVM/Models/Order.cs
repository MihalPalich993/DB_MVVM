using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_MVVM.Models
{
    public partial class Order : ObservableValidator
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        private string _products = string.Empty;
        private decimal _amount;
        private string? _deliveryAddress;

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

        public string? DeliveryAddress
        {
            get => _deliveryAddress;
            set => SetProperty(ref _deliveryAddress, value, true);
        }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        public void Validate() => ValidateAllProperties();
    }
}
