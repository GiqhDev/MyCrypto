using System.Security.Cryptography;
using System.Text;

namespace MyCrypto
{
  public class CryptoHelper
  {
    private static readonly string Key = "M!&CL@V3&SUP3R&S3CR3T@&32&C#@R5!";
    private static readonly string IV = "M!&!V&16&C#@R5!!";

    public static string Encrypt(string plainText)
    {
      using var aes = Aes.Create();
      aes.Key = Encoding.UTF8.GetBytes(Key);
      aes.IV = Encoding.UTF8.GetBytes(IV);
      using var encryptor = aes.CreateEncryptor();
      var bytes = Encoding.UTF8.GetBytes(plainText);
      var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
      return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string cipherText)
    {
      using var aes = Aes.Create();
      aes.Key = Encoding.UTF8.GetBytes(Key);
      aes.IV = Encoding.UTF8.GetBytes(IV);

      using var decryptor = aes.CreateDecryptor();
      var bytes = Convert.FromBase64String(cipherText);
      var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
      return Encoding.UTF8.GetString(decrypted);
    }
  }
}
