# 🔐 MyCrypto

MyCrypto es una librería .NET para cifrado, descifrado y hashing seguro de datos sensibles. La API mantiene un uso simple, puede leer formatos heredados y protege los nuevos payloads cifrados con autenticación.

## 🚀 Características

- 🔒 Cifrado autenticado basado en AES-CBC + HMAC-SHA256.
- 🔓 Compatibilidad de lectura para payloads cifrados con el formato heredado.
- 🔐 Hashing de contraseñas con PBKDF2 y formato versionado.
- ♻️ Verificación compatible con hashes heredados basados en PBKDF2-SHA1.
- ⚡ Hash SHA256 para usos generales que no sean contraseñas.
- ✅ Validador de fortaleza para contraseñas.

## 📥 Instalación

```bash
dotnet add package MyCrypto.GiqhDev
```

## 🛠️ Uso básico

```csharp
using MyCrypto;

byte[] key = CryptoHelper.GenerateKey();
string password = "MiPassword123!";

string encrypted = CryptoHelper.Encrypt(password, key);
string decrypted = CryptoHelper.Decrypt(encrypted, key);

string passwordHash = PasswordHasher.Hash(password);
bool isValidPassword = PasswordHasher.Verify(password, passwordHash);

string sha256 = HashService.ComputeSha256(password);

var (isStrong, errors) = PasswordValidator.Validate(password);
```

## 🧩 Detalles técnicos

- `CryptoHelper.Encrypt` devuelve `v2.{iv}.{cipher}.{tag}`.
- `CryptoHelper.Decrypt` valida primero el `tag` y luego descifra.
- `PasswordHasher.Hash` devuelve `v2.{iterations}.{salt}.{hash}`.
- `PasswordHasher.Verify` acepta formatos heredados para facilitar migraciones.

## 💡 Recomendaciones

- Usa `PasswordHasher` para contraseñas.
- Usa `CryptoHelper` solo cuando realmente necesites recuperar el valor original.
- Guarda las claves fuera del repositorio.
- Rehashea hashes heredados cuando el usuario vuelva a autenticarse correctamente.

## 📦 Compatibilidad

La librería se compila para `net10.0`, `net9.0`, `net8.0`, `net7.0`, `net6.0`, `net5.0` y `netstandard2.0`.

## 🔄 Migración a v3

- `CryptoHelper.Encrypt` ahora escribe `v2.{iv}.{cipher}.{tag}`.
- `CryptoHelper.Decrypt` mantiene compatibilidad con ciphertexts heredados.
- `PasswordHasher.Hash` ahora escribe `v2.{iterations}.{salt}.{hash}`.
- `PasswordHasher.Verify` mantiene compatibilidad con hashes heredados.
- Si dependías del formato exacto generado por las versiones previas, esta versión debe tratarse como una major.
