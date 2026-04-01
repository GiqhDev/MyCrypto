# Changelog

Todos los cambios importantes de este proyecto se documentan en este archivo.

El formato sigue una estructura simple inspirada en Keep a Changelog y versionado semantico.

## [3.0.0] - 2026-04-01

### Added

- Cifrado autenticado para nuevos payloads con formato `v2.{iv}.{cipher}.{tag}`.
- Validacion de integridad antes de descifrar en `CryptoHelper`.
- Compatibilidad de lectura para ciphertexts heredados.
- Versionado explicito para hashes de contrasenas con formato `v2.{iterations}.{salt}.{hash}`.
- Compatibilidad de verificacion para hashes heredados PBKDF2-SHA1.
- Proyecto de pruebas automatizadas para cifrado y hashing.
- Documentacion de migracion a v3.
- Workflow de GitHub Actions para empaquetar y publicar en NuGet mediante tag.

### Changed

- La salida de `CryptoHelper.Encrypt` cambio respecto a versiones anteriores.
- La salida de `PasswordHasher.Hash` cambio respecto a versiones anteriores.
- La descripcion del paquete y la documentacion fueron actualizadas para reflejar el comportamiento actual.
- El ejemplo de consola ahora reutiliza el mismo ciphertext y hash en lugar de regenerarlos en cada llamada.

### Security

- Los nuevos ciphertexts incluyen autenticacion con HMAC-SHA256.
- Se agregaron validaciones defensivas para entradas nulas, vacias o invalidas en la API publica.

## [2.0.0] - Anterior

### Changed

- Se removieron claves e IV hardcodeados.
- Se separaron responsabilidades en `CryptoHelper`, `PasswordHasher`, `HashService` y `PasswordValidator`.
