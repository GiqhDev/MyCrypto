# MyCrypto

MyCrypto es una libreria .NET para cifrado, descifrado y hashing seguro de datos sensibles. La API mantiene un uso simple, puede leer formatos heredados y ahora protege los nuevos payloads cifrados con autenticacion.

## Caracteristicas

- Cifrado autenticado basado en AES-CBC + HMAC-SHA256.
- Compatibilidad de lectura para payloads cifrados con el formato heredado.
- Hashing de contrasenas con PBKDF2 y formato versionado.
- Verificacion compatible con hashes heredados basados en PBKDF2-SHA1.
- Hash SHA256 para usos generales que no sean contrasenas.
- Validador de fortaleza para contrasenas.

## Instalacion

```bash
dotnet add package MyCrypto.GiqhDev
```

## Uso basico

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

## CryptoHelper

- `Encrypt` genera un payload con formato `v2.{iv}.{cipher}.{tag}`.
- `Decrypt` valida el `tag` antes de descifrar.
- `Decrypt` tambien puede leer el formato heredado que almacenaba `IV + ciphertext` en Base64.
- `GenerateKey()` genera 32 bytes aleatorios por defecto.

## PasswordHasher

- `Hash` genera hashes con formato `v2.{iterations}.{salt}.{hash}`.
- `Verify` acepta hashes nuevos con PBKDF2-SHA256 y hashes heredados de 3 segmentos con PBKDF2-SHA1.
- Si tienes hashes heredados, puedes rehashearlos gradualmente cuando el usuario inicie sesion correctamente.

## Buenas practicas

- No uses `HashService` para almacenar contrasenas.
- No hardcodees claves en el codigo.
- Guarda las claves en variables de entorno o gestores de secretos.
- Usa cifrado solo cuando realmente necesites recuperar el valor original.

## Compatibilidad

La libreria se compila para `net10.0`, `net9.0`, `net8.0`, `net7.0`, `net6.0`, `net5.0` y `netstandard2.0`.

## Migracion a v3

- `CryptoHelper.Encrypt` ya no devuelve el Base64 legado de `IV + ciphertext`.
- El nuevo formato de salida es `v2.{iv}.{cipher}.{tag}` y agrega autenticacion del payload.
- `CryptoHelper.Decrypt` sigue pudiendo leer ciphertexts generados por versiones anteriores.
- `PasswordHasher.Hash` ahora genera `v2.{iterations}.{salt}.{hash}`.
- `PasswordHasher.Verify` sigue aceptando hashes heredados de 3 segmentos.
- Si tu aplicacion guarda el valor exacto generado por `Encrypt` o `Hash`, debes asumir que el formato escrito cambió en esta major.

## Validacion

La solucion ahora incluye pruebas automatizadas para:

- Round-trip de cifrado y descifrado.
- Deteccion de payloads manipulados.
- Compatibilidad con ciphertexts heredados.
- Compatibilidad con hashes heredados.
