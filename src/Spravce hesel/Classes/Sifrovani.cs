using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Spravce_hesel.Classes
{
    public class Sifrovani
    {
        //kod prevat z https://gist.github.com/mazhar-ansari-ardeh/d200d91fbafc1af03a0bc0588ef7ffd0

        public static byte[] Zasifrovat(string heslo, byte[] klic)
        {
            byte[] zasifrovano; //deklaruje novy seznam bytu do ktereho se pozdeji ulozi zasiforvane heslo
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider()) //vytvori novou metodu aes
            {
                aes.Key = klic; //nastavi klic pro aes na predem vytvoreny klic
                aes.GenerateIV(); //vygeneruje IV vektor

                aes.Mode = CipherMode.CBC; //nastavi mod aes
                aes.Padding = PaddingMode.PKCS7; //nastavi odsazeni bytu

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);
                    ICryptoTransform encoder = aes.CreateEncryptor();
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encoder, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(heslo);
                    }
                    zasifrovano = msEncrypt.ToArray();
                }
            }

            return zasifrovano;
        }
        
        public static string Desifrovat(byte[] sifra, byte[] klic)
        {
            string desifrovano;
            using(AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {

                //nastaveni aes
                aes.Key = klic;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecryptor = new MemoryStream(sifra))
                {
                    byte[] readIV = new byte[16];
                    msDecryptor.Write(readIV, 0, 16);
                    aes.IV = readIV;
                    ICryptoTransform decoder = aes.CreateDecryptor();
                    using (CryptoStream csDecryptor = new CryptoStream(msDecryptor, decoder, CryptoStreamMode.Read))
                    using (StreamReader srReader = new StreamReader(csDecryptor))
                    {
                        desifrovano = srReader.ReadToEnd();
                    }
                }
            }
            return desifrovano;
        }



        //vlatni kod
        public static byte[] HesloNaKlic(string heslo)
        {
            byte[] bytyHesla = Encoding.UTF8.GetBytes(heslo);

            int delka = bytyHesla.Length;

            if (delka > 256)
            {
                delka = 256;
            }

            byte[] bytes = new byte[256];
            
            for (int i = 0; i < delka; i++)
            {
                bytes[i] = bytyHesla[i];
            }


            int pozice = delka;
            while(delka < 256)
            {
                pozice++;
                bytes[pozice] = 1;
                delka = bytes.Length;
            }


            return bytes;
        }
    }
}
