public static class CurrentUser
{
    public static string Name { get; set; } = "Guest";
    public static string Role { get; set; } = "Guest";
    public static bool IsAdmin => Role == "Admin";
}