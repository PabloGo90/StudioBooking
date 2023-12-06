using System.ComponentModel.DataAnnotations.Schema;
using TattooStudioBooking.Helpers;

namespace TattooStudioBooking;

public class Usuario
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordStored { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    [NotMapped]
    public string Password
    {
        get { return Decrypt(PasswordStored); }
        set { PasswordStored = Encrypt(value); }
    }
    [NotMapped]
    public string? Password2 { get; set; }
    [NotMapped]
    public string? Password3 { get; set; }

    private string Decrypt(string psw)
    {
        Security security = new Security();

        return security.Decrypt("pwd", psw);
    }
    private string Encrypt(string psw)
    {
        Security security = new Security();
        return security.Encrypt("pwd", psw);
    }

}
