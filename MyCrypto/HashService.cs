using System.Security.Cryptography;
using System.Text;

namespace MyCrypto;

/// <summary>
/// Provides fast hashing utilities (e.g., SHA256).
/// ⚠️ Not suitable for password hashing.
/// </summary>
public static class HashService
{
  public static string ComputeSha256(string input)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(input);
    byte[] hash;

#if NET6_0_OR_GREATER
    hash = SHA256.HashData(bytes);
#else
        using var sha256 = SHA256.Create();
        hash = sha256.ComputeHash(bytes);
#endif

    StringBuilder sb = new StringBuilder(hash.Length * 2);

    foreach (var b in hash)
      sb.Append(b.ToString("x2"));

    return sb.ToString();
  }
}