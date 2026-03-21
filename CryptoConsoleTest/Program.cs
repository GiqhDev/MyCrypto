using MyCrypto;

var password = "Q1w2e3r4t5y6";
 var key = CryptoHelper.GenerateKey();
Console.WriteLine(password);
Console.WriteLine("Encrypting...");
Console.WriteLine(CryptoHelper.Encrypt(password,key));
Console.WriteLine("Decrypting...");
Console.WriteLine(CryptoHelper.Decrypt(CryptoHelper.Encrypt(password,key),key));
Console.WriteLine(PasswordHasher.Hash(password));
Console.WriteLine(PasswordHasher.Verify(password, PasswordHasher.Hash(password)));
(bool isValid,List<string> errors ) = PasswordValidator.Validate(password);
if( isValid )
Console.WriteLine(true);
else
{
  Console.WriteLine(false);
  errors.ForEach(e => Console.WriteLine($"- {e}"));
}
Console.WriteLine(HashService.ComputeSha256(password));

Console.ReadKey();
