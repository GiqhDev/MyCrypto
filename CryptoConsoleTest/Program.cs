using MyCrypto;

var password = "Q1w2e3r4t5y6";

Console.WriteLine(password);
Console.WriteLine("Encrypting...");
Console.WriteLine(CryptoHelper.Encrypt(password));
Console.WriteLine("Decrypting...");
Console.WriteLine(CryptoHelper.Decrypt(CryptoHelper.Encrypt(password)));
Console.WriteLine(PasswordHasher.HashPassword(password));
Console.WriteLine(PasswordHasher.VerifyPassword(password, PasswordHasher.HashPassword(password)));
Console.WriteLine(PasswordHasher.CheckPasswordStrength(password));

Console.ReadKey();
