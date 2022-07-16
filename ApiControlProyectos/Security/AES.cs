using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;



namespace ApiControlProyectos.Security
{
    public class AES
    {
        //AES ENCRYPT FROM
        //https://www.siakabaro.com/how-to-perform-aes-encryption-in-net/

        public string AESEncrypt(string plainText)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var conf = builder.Build();

            return Encrypt(plainText, conf["AES:KEY"], conf["AES:IV"]);
        }

        public string AESDecrypt(string cipherText)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var conf = builder.Build();

            return Decrypt(cipherText, conf["AES:KEY"], conf["AES:IV"]);
        }

        private string Encrypt(string plainText, string keyBase64, string vectorBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {

                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);


                //keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
                //vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

                byte[] encryptedData;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encryptedData = ms.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptedData);
            }

        }

        private static string Decrypt(string cipherText, string keyBase64, string vectorBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

                ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                byte[] cipher = Convert.FromBase64String(cipherText);

                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
