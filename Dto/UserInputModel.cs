namespace ApiLabo.Dto;
public class UserInputModel
{
    public string Pseudo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
}
