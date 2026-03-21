using System.Security.Cryptography;
using System.Text;

namespace MyCrypto;

public static class CryptoHelper
{
  public static string Encrypt(string plainText, byte[] key)
  {
    using var aes = Aes.Create();

    aes.Key = key;
    aes.GenerateIV(); // 🔥 IV aleatorio

    using var encryptor = aes.CreateEncryptor();

    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

    // 👉 Guardamos IV + datos cifrados
    byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
    Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

    return Convert.ToBase64String(result);
  }

  public static string Decrypt(string cipherText, byte[] key)
  {
    byte[] fullCipher = Convert.FromBase64String(cipherText);

    using var aes = Aes.Create();
    aes.Key = key;

    // 👉 Extraer IV
    byte[] iv = new byte[16];
    byte[] cipherBytes = new byte[fullCipher.Length - 16];

    Buffer.BlockCopy(fullCipher, 0, iv, 0, 16);
    Buffer.BlockCopy(fullCipher, 16, cipherBytes, 0, cipherBytes.Length);

    aes.IV = iv;

    using var decryptor = aes.CreateDecryptor();
    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

    return Encoding.UTF8.GetString(plainBytes);
  }

  public static byte[] GenerateKey(int size = 32)
  {
    byte[] key = new byte[size];
#if NET6_0_OR_GREATER
    RandomNumberGenerator.Fill(key);
#else
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
#endif
    return key;
  }
}