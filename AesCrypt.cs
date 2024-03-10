using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;


namespace password_manager
{
    public class Aes_Kryptering
    {
        //Skapar en random IV byte array
        public static byte[] GenerateRandomIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }
        
        /*Osäker om vi använder denna längre?
        public static void PrintByteArray(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                Console.Write(b.ToString("x2") + " ");
            }
            Console.WriteLine();
            Console.WriteLine();

        }*/

        //Skapar AES-objekt
        public static Aes CreateAesObject(byte[] vaultKey, byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.Key = vaultKey;
            aes.IV = iv;

            return aes;
        }
    }
}
