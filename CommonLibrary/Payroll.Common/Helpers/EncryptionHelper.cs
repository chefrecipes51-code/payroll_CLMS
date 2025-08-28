using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly byte[] EncryptionKey = new byte[]
        {
            0x45, 0x76, 0x23, 0x91, 0xAB, 0xCD, 0xEF, 0xFE,
            0xBA, 0xDC, 0x98, 0x76, 0x54, 0x32, 0x10, 0xEF,
            0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            0xAB, 0xCD, 0xEF, 0x23, 0x91, 0x45, 0x76, 0x89
        };

        private static readonly byte[] _encryptionKey = EncryptionKey;// Change this key

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _encryptionKey;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _encryptionKey;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                int ivLength = BitConverter.ToInt32(cipherBytes, 0);
                byte[] iv = new byte[ivLength];
                Array.Copy(cipherBytes, sizeof(int), iv, 0, ivLength);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, sizeof(int) + ivLength, cipherBytes.Length - sizeof(int) - ivLength))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
    public static class SingleEncryptionHelper
    {
        private static readonly byte[] EncryptionKey = new byte[]
        {
            0x45, 0x76, 0x23, 0x91, 0xAB, 0xCD, 0xEF, 0xFE,
            0xBA, 0xDC, 0x98, 0x76, 0x54, 0x32, 0x10, 0xEF,
            0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            0xAB, 0xCD, 0xEF, 0x23, 0x91, 0x45, 0x76, 0x89
        };

        private static readonly byte[] _encryptionKey = EncryptionKey[..32]; // Ensure key is exactly 32 bytes

        public static string Encrypt(string plainText)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _encryptionKey;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.GenerateIV(); // Generates a unique IV for each encryption

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Prepend IV to the data

                        using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption failed", ex);
            }
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _encryptionKey;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    byte[] iv = new byte[16]; // AES uses a 16-byte IV
                    Array.Copy(cipherBytes, 0, iv, 0, iv.Length); // Extract IV

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption failed", ex);
            }
        }
    }
}
