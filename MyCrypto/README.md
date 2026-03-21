# 🔐 MyCrypto

MyCrypto es una librería .NET diseñada para el cifrado, descifrado y hashing seguro de datos sensibles como contraseñas y claves, proporcionando una interfaz simple y siguiendo buenas prácticas criptográficas.

---

## 🚀 Características

* 🔒 Encriptación segura con AES (IV aleatorio)
* 🔓 Desencriptación sencilla
* 🔐 Hashing seguro de contraseñas (PBKDF2 + salt)
* ⚡ Hashing rápido (SHA256) para usos generales
* ✅ Validación de fortaleza de contraseñas
* 🧩 Integración rápida en cualquier proyecto .NET
* 📦 Compatible con múltiples versiones de .NET

---

## 📥 Instalación

```bash
dotnet add package MyCrypto
```

---

## 🧠 Componentes principales

* **CryptoHelper** → Encriptación y desencriptación (AES con IV dinámico)
* **PasswordHasher** → Hashing seguro de contraseñas (PBKDF2)
* **HashService** → Hashing rápido (SHA256, no para contraseñas)
* **PasswordValidator** → Validación de fortaleza de contraseñas

---

## 🛠️ Uso básico

```csharp
using MyCrypto;

// 🔑 Generar una clave segura (32 bytes = AES-256)
byte[] key = CryptoHelper.GenerateKey();

string password = "MiPassword123!";

// 🔒 Encriptar / Desencriptar
string encrypted = CryptoHelper.Encrypt(password, key);
string decrypted = CryptoHelper.Decrypt(encrypted, key);

// 🔐 Hashing seguro de contraseñas
string hashed = PasswordHasher.Hash(password);
bool isValid = PasswordHasher.Verify(password, hashed);

// ⚡ Hash rápido (NO usar para contraseñas)
string sha256 = HashService.ComputeSha256(password);

// ✅ Validación de seguridad
var (isStrong, errors) = PasswordValidator.Validate(password);
```

---

## 🔐 Cómo funciona la encriptación

* Se utiliza **AES**
* El **IV se genera automáticamente en cada operación**
* El IV se almacena junto con el texto cifrado
* La clave (**key**) debe ser proporcionada por el usuario

---

## ⚠️ Buenas prácticas

* No almacenar contraseñas en texto plano
* Usar hashing seguro (PBKDF2) para contraseñas
* Usar cifrado solo cuando necesites recuperar el valor original
* **Nunca hardcodear claves en el código**
* Guardar claves en:

  * Variables de entorno
  * Azure Key Vault u otros gestores de secretos
* Mantener la librería actualizada

---

## 🔐 Consideraciones de seguridad

* El hashing de contraseñas es unidireccional (no se puede desencriptar)
* **No uses SHA256 para contraseñas**, utiliza `PasswordHasher`
* El IV es generado automáticamente para mayor seguridad
* Esta librería no reemplaza una correcta gestión de secretos

---

## ⚠️ Breaking Changes (v2.0.0)

Esta versión introduce cambios incompatibles con versiones anteriores:

* Se eliminaron claves e IV hardcodeados en `CryptoHelper`
* Ahora la clave debe ser proporcionada por el usuario
* Se implementa IV dinámico para mejorar la seguridad
* Se renombraron métodos:

  * `HashPassword` → `Hash`
  * `VerifyPassword` → `Verify`
* Se separaron responsabilidades en nuevas clases:

  * `HashService` para hashing rápido (SHA256)
  * `PasswordValidator` para validación de contraseñas

👉 Si vienes de versiones anteriores, deberás actualizar el uso de la API.

---

## 📌 Versión

Versión actual: **2.0.0**

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Puedes abrir un issue o enviar un pull request.

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Consulta el archivo LICENSE para más detalles.
