using System.Security.Cryptography;

namespace MyCrypto;

public static class PasswordHasher
{
  private const int SaltSize = 16;
  private const int HashSize = 32;
  private const int Iterations = 100_000;

  /// <summary>
  /// Format: {iterations}.{salt}.{hash}
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

    byte[] hash = DeriveKey(password, salt, Iterations, HashSize);

    return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
  }

  public static bool Verify(string password, string storedHash)
  {
    if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
      return false;

    var parts = storedHash.Split('.');
    if (parts.Length != 3) return false;

    if (!int.TryParse(parts[0], out int iterations)) return false;

    try
    {
      byte[] salt = Convert.FromBase64String(parts[1]);
      byte[] hash = Convert.FromBase64String(parts[2]);

      byte[] testHash = DeriveKey(password, salt, iterations, hash.Length);

      return FixedTimeEquals(testHash, hash);
    }
    catch
    {
      return false;
    }
  }

  private static byte[] DeriveKey(string password, byte[] salt, int iterations, int outputBytes)
  {
#if NET6_0_OR_GREATER
    return Rfc2898DeriveBytes.Pbkdf2(
        password,
        salt,
        iterations,
        HashAlgorithmName.SHA256,
        outputBytes);
#else
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
        {
            return pbkdf2.GetBytes(outputBytes);
        }
#endif
  }

  private static bool FixedTimeEquals(byte[] a, byte[] b)
  {
    if (a.Length != b.Length) return false;

    int diff = 0;
    for (int i = 0; i < a.Length; i++)
      diff |= a[i] ^ b[i];

    return diff == 0;
  }
}