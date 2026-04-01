using System.Security.Cryptography;
using Xunit;

namespace MyCrypto.Tests;

public class PasswordHasherTests
{
  [Fact]
  public void Hash_UsesVersionedFormat()
  {
    string hash = PasswordHasher.Hash("Sup3rSecure!");

    Assert.StartsWith("v2.", hash, StringComparison.Ordinal);
  }

  [Fact]
  public void Verify_AcceptsCurrentHashes()
  {
    const string password = "Sup3rSecure!";
    string hash = PasswordHasher.Hash(password);

    Assert.True(PasswordHasher.Verify(password, hash));
  }

  [Fact]
  public void Verify_AcceptsLegacySha1Hashes()
  {
    const string password = "LegacyPassword123!";
    string hash = HashLegacy(password);

    Assert.True(PasswordHasher.Verify(password, hash));
  }

  [Fact]
  public void Verify_ReturnsFalseForInvalidPayload()
  {
    Assert.False(PasswordHasher.Verify("anything", "invalid"));
  }

  private static string HashLegacy(string password)
  {
    byte[] salt = new byte[16];
    RandomNumberGenerator.Fill(salt);
    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
    byte[] hash = Pbkdf2Sha1(passwordBytes, salt, 100_000, 32);

    return $"100000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
  }

  private static byte[] Pbkdf2Sha1(byte[] password, byte[] salt, int iterations, int outputBytes)
  {
    using var hmac = new HMACSHA1(password);

    int hashLength = hmac.HashSize / 8;
    int blockCount = (int)Math.Ceiling(outputBytes / (double)hashLength);
    byte[] output = new byte[outputBytes];
    byte[] saltBuffer = new byte[salt.Length + 4];
    Buffer.BlockCopy(salt, 0, saltBuffer, 0, salt.Length);

    int outputOffset = 0;
    for (int blockIndex = 1; blockIndex <= blockCount; blockIndex++)
    {
      saltBuffer[salt.Length] = (byte)(blockIndex >> 24);
      saltBuffer[salt.Length + 1] = (byte)(blockIndex >> 16);
      saltBuffer[salt.Length + 2] = (byte)(blockIndex >> 8);
      saltBuffer[salt.Length + 3] = (byte)blockIndex;

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
}
