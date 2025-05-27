namespace CoreFinance.Contracts.DTOs;

public class LoginDto
{
    public LoginDto()
    {
    }

    public LoginDto(string? userName, string? password)
    {
        UserName = userName;
        Password = password;
    }

    public string? UserName { get; set; }
    public string? Password { get; set; }
}