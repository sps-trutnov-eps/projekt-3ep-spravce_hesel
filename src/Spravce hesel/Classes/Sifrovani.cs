using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Spravce_hesel.Classes
{
    public static class Sifrovani
    {
        //kod prevat z https://learn.microsoft.com/cs-cz/dotnet/api/system.security.cryptography.aes?view=net-6.0

        public static byte[] Zasifrovat(string plainText, byte[] key, byte[] iv)
        {
            try {
                // Check arguments.
                if (plainText == null || plainText.Length <= 0)
                    throw new ArgumentNullException("plainText");
                if (key == null || key.Length <= 0)
                    throw new ArgumentNullException("key");
                if (iv == null || iv.Length <= 0)
                    throw new ArgumentNullException("iv");
                byte[] encrypted;

                // Create an Aes object
                // with the specified key and IV.
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new())
                    {
                        using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                // Return the encrypted bytes from the memory stream.
                return encrypted;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return null;
            }

        }

        public static string Desifrovat(byte[] cipherText, byte[] key, byte[] iv)
        {
            try
            {
                // Check arguments.
                if (cipherText == null || cipherText.Length <= 0)
                    throw new ArgumentNullException("cipherText");
                if (key == null || key.Length <= 0)
                    throw new ArgumentNullException("key");
                if (iv == null || iv.Length <= 0)
                    throw new ArgumentNullException("iv");

                // Declare the string used to hold
                // the decrypted text.
                string? plaintext = null;

                // Create an Aes object
                // with the specified key and IV.
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new(cipherText))
                    {
                        using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return null;
            }
        }
        
        // Vlastn� k�d
        public static byte[] HesloNaKlic(string heslo)
        {
            byte[] bytyHesla = Encoding.UTF8.GetBytes(heslo);

            int delka = bytyHesla.Length;

            if (delka > 32)
            {
                delka = 32;
            }

            byte[] bytes = new byte[32];

            for (int i = 0; i < delka; i++)
            {
                bytes[i] = bytyHesla[i];
            }

            int pozice = delka;
            while (delka < 32)
            {
                pozice++;
                bytes[pozice] = 1;
                delka = bytes.Length;
            }

            return bytes;
        }
        
        public static string InfoProKlic(int length)
        {
            Random random = new();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
