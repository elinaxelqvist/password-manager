using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;


namespace password_manager
{
    public class Aes_Kryptering
    {


        public static byte[] GenerateRandomIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }

        public static void PrintByteArray(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                Console.Write(b.ToString("x2") + " ");
            }
            Console.WriteLine();
            Console.WriteLine();

        }
        //test 
    }
}
