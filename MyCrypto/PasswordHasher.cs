using System.Security.Cryptography;

namespace MyCrypto;

public static class PasswordHasher
{
  private const string CurrentHashVersion = "v2";
  private const int SaltSize = 16;
  private const int HashSize = 32;
  private const int Iterations = 100_000;

  /// <summary>
  /// Format: v2.{iterations}.{salt}.{hash}
  /// </summary>
  public static string Hash(string password)
  {
    if (string.IsNullOrWhiteSpace(password))
      throw new ArgumentException("Password cannot be empty", nameof(password));

    byte[] salt = new byte[SaltSize];

#if NET6_0_OR_GREATER
    RandomNumberGenerator.Fill(salt);
#else
    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(salt);
    }
#endif

    byte[] hash = DeriveKey(password, salt, Iterations, HashSize, HashAlgorithmName.SHA256);

    return $"{CurrentHashVersion}.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
  }

  public static bool Verify(string password, string storedHash)
  {
    if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
      return false;

    var parts = storedHash.Split('.');

    try
    {
      if (parts.Length == 4 && string.Equals(parts[0], CurrentHashVersion, StringComparison.Ordinal))
      {
        return VerifyPassword(password, parts[1], parts[2], parts[3], HashAlgorithmName.SHA256);
      }

      if (parts.Length == 3)
      {
        return VerifyPassword(password, parts[0], parts[1], parts[2], HashAlgorithmName.SHA1);
      }

      return false;
    }
    catch
    {
      return false;
    }
  }

  private static bool VerifyPassword(
    string password,
    string iterationsValue,
    string saltValue,
    string hashValue,
    HashAlgorithmName algorithm)
  {
    if (!int.TryParse(iterationsValue, out int iterations) || iterations <= 0)
      return false;

    byte[] salt = Convert.FromBase64String(saltValue);
    byte[] hash = Convert.FromBase64String(hashValue);
    byte[] testHash = DeriveKey(password, salt, iterations, hash.Length, algorithm);

    return FixedTimeEquals(testHash, hash);
  }

  private static byte[] DeriveKey(
    string password,
    byte[] salt,
    int iterations,
    int outputBytes,
    HashAlgorithmName algorithm)
  {
#if NET6_0_OR_GREATER
    return Rfc2898DeriveBytes.Pbkdf2(
      password,
      salt,
      iterations,
      algorithm,
      outputBytes);
#else
    return Pbkdf2Compat(password, salt, iterations, outputBytes, algorithm);
#endif
  }

#if !NET6_0_OR_GREATER
  private static byte[] Pbkdf2Compat(
    string password,
    byte[] salt,
    int iterations,
    int outputBytes,
    HashAlgorithmName algorithm)
  {
    if (iterations <= 0)
      throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations must be greater than zero.");

    if (outputBytes <= 0)
      throw new ArgumentOutOfRangeException(nameof(outputBytes), "Output length must be greater than zero.");

    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
    using var hmac = CreateHmac(passwordBytes, algorithm);

    int hashLength = hmac.HashSize / 8;
    int blockCount = (int)Math.Ceiling(outputBytes / (double)hashLength);
    byte[] output = new byte[outputBytes];
    byte[] saltBuffer = new byte[salt.Length + 4];
    Buffer.BlockCopy(salt, 0, saltBuffer, 0, salt.Length);

    int outputOffset = 0;
    for (int blockIndex = 1; blockIndex <= blockCount; blockIndex++)
    {
      WriteBlockIndex(saltBuffer, salt.Length, blockIndex);

      byte[] block = hmac.ComputeHash(saltBuffer);
      byte[] accumulator = (byte[])block.Clone();

      for (int iteration = 1; iteration < iterations; iteration++)
      {
        block = hmac.ComputeHash(block);
        for (int i = 0; i < accumulator.Length; i++)
          accumulator[i] ^= block[i];
      }

      int bytesToCopy = Math.Min(hashLength, outputBytes - outputOffset);
      Buffer.BlockCopy(accumulator, 0, output, outputOffset, bytesToCopy);
      outputOffset += bytesToCopy;
    }

    return output;
  }

  private static HMAC CreateHmac(byte[] key, HashAlgorithmName algorithm)
  {
    if (algorithm == HashAlgorithmName.SHA1) return new HMACSHA1(key);
    if (algorithm == HashAlgorithmName.SHA256) return new HMACSHA256(key);

    throw new CryptographicException($"Hash algorithm '{algorithm.Name}' is not supported.");
  }

  private static void WriteBlockIndex(byte[] buffer, int offset, int value)
  {
    buffer[offset] = (byte)(value >> 24);
    buffer[offset + 1] = (byte)(value >> 16);
    buffer[offset + 2] = (byte)(value >> 8);
    buffer[offset + 3] = (byte)value;
  }
#endif

  private static bool FixedTimeEquals(byte[] a, byte[] b)
  {
    if (a.Length != b.Length) return false;

    int diff = 0;
    for (int i = 0; i < a.Length; i++)
      diff |= a[i] ^ b[i];

    return diff == 0;
  }
}
