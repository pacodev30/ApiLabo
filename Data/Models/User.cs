namespace ApiLabo.Data.Models;
public class User
{
    public int Id { get; set; }
    public string Pseudo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string DisplayId { get; set; } = string.Empty;
}
