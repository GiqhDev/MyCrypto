using MyCrypto;
using System;
namespace MyCryptoConsoleNetFrameworkTest
{
  internal class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine(PasswordHasher.Hash("q1w2e3r4t5y6"));
      Console.WriteLine(PasswordHasher.Verify("q1w2e3r4t5y6", PasswordHasher.Hash("q1w2e3r4t5y6")));
      Console.ReadKey();
    }
  }
}
