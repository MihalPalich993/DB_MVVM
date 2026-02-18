using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_MVVM.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        // Дата заказа. По умолчанию - сейчас.
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Описание товаров (для простоты - строкой)
        [Required]
        public string Products { get; set; } = string.Empty;

        // Сумма заказа (decimal - стандарт для денег)
        [Column(TypeName = "decimal(18,2)")] // Настройка точности для БД
        public decimal Amount { get; set; }

        // --- ВНЕШНИЙ КЛЮЧ (Связь с клиентом) ---

        // 1. Ссылка на ID клиента
        public int ClientId { get; set; }

        // 2. Ссылка на самого клиента (Навигационное свойство)
        // ForeignKey говорит, что это поле связано через ClientId
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
}