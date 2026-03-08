public class LogEntry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now; // Когда
    public string Username { get; set; } = "System";      // Кто
    public string Action { get; set; } = string.Empty;     // Что сделал (Добавил/Удалил)
    public string EntityName { get; set; } = string.Empty; // Над кем (Клиент/Заказ)
}