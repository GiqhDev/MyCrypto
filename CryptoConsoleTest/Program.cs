using MyCrypto;

var password = "Q1w2e3r4t5y6";
var key = CryptoHelper.GenerateKey();
string encrypted = CryptoHelper.Encrypt(password, key);
string passwordHash = PasswordHasher.Hash(password);

Console.WriteLine(password);
Console.WriteLine("Encrypting...");
Console.WriteLine(encrypted);
Console.WriteLine("Decrypting...");
Console.WriteLine(CryptoHelper.Decrypt(encrypted, key));
Console.WriteLine(passwordHash);
Console.WriteLine(PasswordHasher.Verify(password, passwordHash));

(bool isValid, List<string> errors) = PasswordValidator.Validate(password);
if (isValid)
  Console.WriteLine(true);
else
{
  Console.WriteLine(false);
  errors.ForEach(e => Console.WriteLine($"- {e}"));
}

Console.WriteLine(HashService.ComputeSha256(password));

Console.ReadKey();
