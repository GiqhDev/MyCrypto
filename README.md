# 🔐 MyCrypto

MyCrypto es una librería .NET diseñada para el cifrado, descifrado y hashing seguro de datos sensibles como contraseñas y claves, proporcionando una interfaz simple y siguiendo buenas prácticas criptográficas.

---

## 🚀 Características

* 🔒 Encriptación segura de datos
* 🔓 Desencriptación sencilla
* 🔐 Hashing seguro de contraseñas
* ✅ Validación de fortaleza de contraseñas
* 🧩 Integración rápida en cualquier proyecto .NET
* 📦 Compatible con .NET moderno

---

## 📥 Instalación

```bash
dotnet add package MyCrypto
```

---

## 🧠 Componentes principales

* **CryptoHelper** → Encriptación y desencriptación
* **PasswordHasher** → Hashing y validación de contraseñas

---

## 🛠️ Uso básico

```csharp
using MyCrypto;

string password = "MiPassword123!";

// 🔒 Encriptar / Desencriptar
string encrypted = CryptoHelper.Encrypt(password);
string decrypted = CryptoHelper.Decrypt(encrypted);

// 🔐 Hashing de contraseñas
string hashed = PasswordHasher.HashPassword(password);
bool isValid = PasswordHasher.VerifyPassword(password, hashed);

// ✅ Validación de seguridad
bool isStrong = PasswordHasher.CheckPasswordStrength(password);
```

---

## ⚠️ Buenas prácticas

* No almacenar claves en texto plano
* Usar variables de entorno para secretos
* Mantener actualizada la librería

---

## 🔐 Consideraciones de seguridad

* El hashing es unidireccional (no se puede desencriptar)
* Usa hashing para contraseñas, no cifrado
* Protege adecuadamente las claves de cifrado
* Esta librería no reemplaza una correcta gestión de secretos (ej: Azure Key Vault, variables de entorno)

---

## 📌 Versión

Versión actual: 1.0.0

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Puedes abrir un issue o enviar un pull request.

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Consulta el archivo LICENSE para más detalles.
