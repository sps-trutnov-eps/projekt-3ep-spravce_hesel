using NuGet.Packaging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Spravce_hesel.Classes
{
    public class Sifrovani
    {
        //Kod prevzat z https://www.c-sharpcorner.com/article/encryption-and-decryption-using-a-symmetric-key-in-c-sharp/

        public static string Zasifrovat(string Klic, string Text)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Klic);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(Text);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }


                return Convert.ToBase64String(array);
        }

        public static string Desifrovat(string Klic, string Sifra)
        {

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(Sifra);

            

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Klic);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }

        }

        public static string HesloNaKlic(string heslo)
        {
            byte[] bytyHesla = Encoding.ASCII.GetBytes(heslo);

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
            while(delka < 32)
            {
                pozice++;
                bytes[pozice] = 1;
                delka = bytes.Length;
            }


            string klic = Encoding.ASCII.GetString(bytes);


            return klic;
        }
    }
}
