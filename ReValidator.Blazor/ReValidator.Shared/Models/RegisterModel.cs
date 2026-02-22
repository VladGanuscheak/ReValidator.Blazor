namespace ReValidator.Shared.Models;

public class RegisterModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool AcceptTerms { get; set; }
}
