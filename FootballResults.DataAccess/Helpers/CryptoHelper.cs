using System.Security.Cryptography;
using System.Text;

namespace FootballResults.WebApp.Services.Helpers
{
    public static class CryptoHelper
    {
        private static byte[] DEFAULT_IV = new byte[]
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
        };

        private static byte[] CreateKey(string data)
        {
            byte[] salt = Array.Empty<byte>();
            int iterations = 1000;
            HashAlgorithmName method = HashAlgorithmName.SHA384;
            int desiredKeyLength = 16;

            return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(data), salt,
                iterations, method, desiredKeyLength);
        }

        public static async Task<byte[]> EncryptAsync(string data, string key)
        {
            if (data == null)
                return null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = CreateKey(key ?? data);
                aes.IV = DEFAULT_IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] dataBytes = Encoding.Unicode.GetBytes(data);
                        await cryptoStream.WriteAsync(dataBytes, 0, dataBytes.Length);
                    }

                    return memoryStream.ToArray();
                }
            }
        }

        public static async Task<string> DecryptAsync(byte[] encryptedData, string key)
        {
            if (encryptedData == null)
                return null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = CreateKey(key);
                aes.IV = DEFAULT_IV;

                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            await cryptoStream.CopyToAsync(output);
                            return Encoding.Unicode.GetString(output.ToArray());
                        }
                    }
                }
            }
        }
    }
}
