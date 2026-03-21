using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyCrypto
{
  public class PasswordHasher 
  {
    public static string HashPassword(string password)
    {
      ASCIIEncoding encoding = new();
      StringBuilder sb = new();
      byte[] stream = SHA256.HashData(encoding.GetBytes(password));
      for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
      return sb.ToString();
    }

    public static bool VerifyPassword(string password, string base64Hash)
    {
      return HashPassword(password).Equals(base64Hash);
    }

    public static string CheckPasswordStrength(string password)
    {
      StringBuilder sb = new();
      if (password.Length < 8)
      {
        sb.Append("La contraseña debe tener minimo 8 caracteres. " + Environment.NewLine);
      }

      if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
      {
        sb.Append("La contraseña debe contener mayusculas, minusculas y numeros " + Environment.NewLine);
      }
      return sb.ToString();
    }
  }
}
