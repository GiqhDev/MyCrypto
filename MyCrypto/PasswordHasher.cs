using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyCrypto
{
  public class PasswordHasher 
  {
    public static string HashPassword(string password)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(password);
      byte[] hash = null;

#if NET6_0_OR_GREATER
       hash = SHA256.HashData(bytes);
#else
    using (var sha256 = SHA256.Create())
    {
        hash = sha256.ComputeHash(bytes);
    }
#endif

      StringBuilder sb = new StringBuilder(hash.Length * 2);
      for (int i = 0; i < hash.Length; i++)
      {
        sb.Append(hash[i].ToString("x2"));
      }

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
