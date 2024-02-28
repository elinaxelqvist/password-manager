using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace password_manager
{
    public class Vault
    {


        public static void ServerFileStructure(string filePath, string iv, string password)
        {
            // Skapa en dictionary där nyckeln är initieringsvektorn och värdet är en lista av lösenord
            Dictionary<string, List<string>> vaultStructure = new Dictionary<string, List<string>>();

            // Kontrollera om filen redan finns och ladda innehållet om den gör det
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                vaultStructure = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
            }

            // Lägg till lösenord i valvet
            AddPassword(vaultStructure, iv, password);

            // Konvertera till JSON-sträng och spara i filen
            string jsonOutput = JsonSerializer.Serialize(vaultStructure, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonOutput);

            Console.WriteLine("Lösenord tillagt i serverfilen.");
        }

        // Metod för att lägga till ett lösenord i valvet
        public static void AddPassword(Dictionary<string, List<string>> vaultStructure, string iv, string password)
        {
            if (!vaultStructure.ContainsKey(iv))
            {
                vaultStructure[iv] = new List<string>();
            }

            vaultStructure[iv].Add(password);
        }
    }
}


        
           


        
            
        
        

        








    

