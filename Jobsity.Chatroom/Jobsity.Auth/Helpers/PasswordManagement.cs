using FluentValidation.Results;
using Jobsity.Auth.DTOs;
using System.Security.Cryptography;
using System.Text;
using static Jobsity.Auth.Validators.AccountValidator;

namespace Jobsity.Auth.Helpers
{
    public static class PasswordManagement
    {
        public static string DecodePassword(string encodedPassword)
        {
            byte[] buffer = Convert.FromBase64String(encodedPassword);
            string key = "J0bsityC1@sr00m3";

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(key);
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static ValidationResult FluentValidatePassword(string decodedPassword, string decodedConfirmPassword)
        {
            PasswordDTO passwordDTO = new PasswordDTO();
            passwordDTO.Password = decodedPassword;
            passwordDTO.ConfirmPassword = decodedConfirmPassword;
            PasswordValidator validator = new PasswordValidator();
            ValidationResult validationResult = validator.Validate(passwordDTO);
            return validationResult;
        }

        public static Dictionary<string, List<string>> GetErrorObjectFromValidationResult(ValidationResult validationResult)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            string propertyName = "";
            List<string> errorMessages = new List<string>();
            foreach (var error in validationResult.Errors)
            {
                if (String.IsNullOrEmpty(propertyName))
                {
                    propertyName = error.PropertyName;
                }
                else if (propertyName != error.PropertyName)
                {
                    errors.Add(propertyName, errorMessages);
                    propertyName = error.PropertyName;
                    errorMessages = new List<string>();
                }
                errorMessages.Add(error.ErrorMessage);
            }
            errors.Add(propertyName, errorMessages);
            return errors;
        }

    }
}
