using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace MyCrypto.Tests;

public class CryptoHelperTests
{
  [Fact]
  public void EncryptAndDecrypt_RoundTripsUsingCurrentFormat()
  {
    byte[] key = CryptoHelper.GenerateKey();
    const string plainText = "Mensaje secreto 123!";

    string cipherText = CryptoHelper.Encrypt(plainText, key);
    string decrypted = CryptoHelper.Decrypt(cipherText, key);

    Assert.StartsWith("v2.", cipherText, StringComparison.Ordinal);
    Assert.Equal(plainText, decrypted);
  }

  [Fact]
  public void Encrypt_UsesRandomIvForDifferentPayloads()
  {
    byte[] key = CryptoHelper.GenerateKey();
    const string plainText = "same message";

    string first = CryptoHelper.Encrypt(plainText, key);
    string second = CryptoHelper.Encrypt(plainText, key);

    Assert.NotEqual(first, second);
  }

  [Fact]
  public void Decrypt_AcceptsLegacyPayloads()
  {
    byte[] key = CryptoHelper.GenerateKey();
    const string plainText = "legacy payload";
    string legacyCipherText = EncryptLegacy(plainText, key);

    string decrypted = CryptoHelper.Decrypt(legacyCipherText, key);

    Assert.Equal(plainText, decrypted);
  }

  [Fact]
  public void Decrypt_RejectsTamperedPayloads()
  {
    byte[] key = CryptoHelper.GenerateKey();
    string cipherText = CryptoHelper.Encrypt("important", key);
    string[] parts = cipherText.Split('.');
    byte[] tag = Convert.FromBase64String(parts[3]);
    tag[0] ^= 0xFF;
    parts[3] = Convert.ToBase64String(tag);

    Assert.Throws<CryptographicException>(() => CryptoHelper.Decrypt(string.Join(".", parts), key));
  }

  [Fact]
  public void Encrypt_RejectsEmptyKey()
  {
    Assert.Throws<ArgumentException>(() => CryptoHelper.Encrypt("hi", Array.Empty<byte>()));
  }

  private static string EncryptLegacy(string plainText, byte[] key)
  {
    using var aes = Aes.Create();
    aes.Key = key;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;
    aes.GenerateIV();

    using var encryptor = aes.CreateEncryptor();
    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

    byte[] payload = new byte[aes.IV.Length + cipherBytes.Length];
    Buffer.BlockCopy(aes.IV, 0, payload, 0, aes.IV.Length);
    Buffer.BlockCopy(cipherBytes, 0, payload, aes.IV.Length, cipherBytes.Length);

    return Convert.ToBase64String(payload);
  }
}
