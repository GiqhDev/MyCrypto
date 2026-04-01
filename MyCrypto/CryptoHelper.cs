using System.Security.Cryptography;
using System.Text;

namespace MyCrypto;

public static class CryptoHelper
{
  private const string CurrentPayloadVersion = "v2";
  private const int IvSize = 16;
  private const int TagSize = 32;

  public static string Encrypt(string plainText, byte[] key)
  {
    if (plainText is null)
      throw new ArgumentNullException(nameof(plainText));

    var (encryptionKey, authenticationKey) = DeriveSubkeys(key);

    using var aes = Aes.Create();
    aes.Key = encryptionKey;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;
    aes.GenerateIV();

    using var encryptor = aes.CreateEncryptor();
    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
    byte[] tag = ComputeAuthenticationTag(authenticationKey, aes.IV, cipherBytes);

    return string.Join(
      ".",
      CurrentPayloadVersion,
      Convert.ToBase64String(aes.IV),
      Convert.ToBase64String(cipherBytes),
      Convert.ToBase64String(tag));
  }

  public static string Decrypt(string cipherText, byte[] key)
  {
    if (string.IsNullOrWhiteSpace(cipherText))
      throw new ArgumentException("Cipher text cannot be null or empty.", nameof(cipherText));

    return cipherText.StartsWith($"{CurrentPayloadVersion}.", StringComparison.Ordinal)
      ? DecryptVersion2(cipherText, key)
      : DecryptLegacy(cipherText, key);
  }

  public static byte[] GenerateKey(int size = 32)
  {
    if (size <= 0)
      throw new ArgumentOutOfRangeException(nameof(size), "Key size must be greater than zero.");

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

  private static string DecryptVersion2(string cipherText, byte[] key)
  {
    var (encryptionKey, authenticationKey) = DeriveSubkeys(key);
    var parts = cipherText.Split('.');

    if (parts.Length != 4 || !string.Equals(parts[0], CurrentPayloadVersion, StringComparison.Ordinal))
      throw new FormatException("Cipher text payload is not in a supported format.");

    byte[] iv = Convert.FromBase64String(parts[1]);
    byte[] cipherBytes = Convert.FromBase64String(parts[2]);
    byte[] providedTag = Convert.FromBase64String(parts[3]);

    if (iv.Length != IvSize)
      throw new CryptographicException("Cipher text payload contains an invalid IV.");

    if (providedTag.Length != TagSize)
      throw new CryptographicException("Cipher text payload contains an invalid authentication tag.");

    byte[] expectedTag = ComputeAuthenticationTag(authenticationKey, iv, cipherBytes);
    if (!FixedTimeEquals(expectedTag, providedTag))
      throw new CryptographicException("Cipher text authentication failed.");

    using var aes = Aes.Create();
    aes.Key = encryptionKey;
    aes.IV = iv;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;

    using var decryptor = aes.CreateDecryptor();
    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

    return Encoding.UTF8.GetString(plainBytes);
  }

  private static string DecryptLegacy(string cipherText, byte[] key)
  {
    ValidateKeyMaterial(key);

    byte[] fullCipher = Convert.FromBase64String(cipherText);
    if (fullCipher.Length <= IvSize)
      throw new CryptographicException("Cipher text payload is too short.");

    using var aes = Aes.Create();
    aes.Key = key;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;

    byte[] iv = new byte[IvSize];
    byte[] cipherBytes = new byte[fullCipher.Length - IvSize];

    Buffer.BlockCopy(fullCipher, 0, iv, 0, IvSize);
    Buffer.BlockCopy(fullCipher, IvSize, cipherBytes, 0, cipherBytes.Length);

    aes.IV = iv;

    using var decryptor = aes.CreateDecryptor();
    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

    return Encoding.UTF8.GetString(plainBytes);
  }

  private static (byte[] EncryptionKey, byte[] AuthenticationKey) DeriveSubkeys(byte[] key)
  {
    ValidateKeyMaterial(key);

    byte[] keyMaterial = ComputeSha512(key);
    byte[] encryptionKey = new byte[32];
    byte[] authenticationKey = new byte[32];

    Buffer.BlockCopy(keyMaterial, 0, encryptionKey, 0, encryptionKey.Length);
    Buffer.BlockCopy(keyMaterial, encryptionKey.Length, authenticationKey, 0, authenticationKey.Length);

    return (encryptionKey, authenticationKey);
  }

  private static void ValidateKeyMaterial(byte[] key)
  {
    if (key is null)
      throw new ArgumentNullException(nameof(key));

    if (key.Length == 0)
      throw new ArgumentException("Key cannot be empty.", nameof(key));
  }

  private static byte[] ComputeAuthenticationTag(byte[] authenticationKey, byte[] iv, byte[] cipherBytes)
  {
    byte[] payload = new byte[iv.Length + cipherBytes.Length];
    Buffer.BlockCopy(iv, 0, payload, 0, iv.Length);
    Buffer.BlockCopy(cipherBytes, 0, payload, iv.Length, cipherBytes.Length);

    using var hmac = new HMACSHA256(authenticationKey);
    return hmac.ComputeHash(payload);
  }

  private static byte[] ComputeSha512(byte[] value)
  {
#if NET6_0_OR_GREATER
    return SHA512.HashData(value);
#else
    using (var sha512 = SHA512.Create())
    {
      return sha512.ComputeHash(value);
    }
#endif
  }

  private static bool FixedTimeEquals(byte[] left, byte[] right)
  {
    if (left.Length != right.Length) return false;

    int diff = 0;
    for (int i = 0; i < left.Length; i++)
      diff |= left[i] ^ right[i];

    return diff == 0;
  }
}
