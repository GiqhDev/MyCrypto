namespace MyCrypto;

public static class PasswordValidator
{
  public static (bool IsValid, List<string> Errors) Validate(
      string password,
      int minLength = 10,
      int minUpper = 1,
      int minLower = 1,
      int minDigits = 1,
      int minSpecial = 1)
  {
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(password))
    {
      errors.Add("La contraseña no puede estar vacía.");
      return (false, errors);
    }

    if (password.Length < minLength)
      errors.Add($"La contraseña debe tener al menos {minLength} caracteres.");

    int upper = password.Count(char.IsUpper);
    int lower = password.Count(char.IsLower);
    int digits = password.Count(char.IsDigit);
    int special = password.Count(c => !char.IsLetterOrDigit(c));

    if (upper < minUpper) errors.Add($"Debe contener al menos {minUpper} mayúscula(s).");
    if (lower < minLower) errors.Add($"Debe contener al menos {minLower} minúscula(s).");
    if (digits < minDigits) errors.Add($"Debe contener al menos {minDigits} número(s).");
    if (special < minSpecial) errors.Add($"Debe contener al menos {minSpecial} carácter(es) especial(es).");

    string lowerPwd = password.ToLowerInvariant();
    var banned = new[] { "123456", "password", "qwerty", "admin", "12345678", "letmein" };

    if (banned.Any(b => lowerPwd.Contains(b)))
      errors.Add("La contraseña es demasiado común o insegura.");

    return (errors.Count == 0, errors);
  }
}