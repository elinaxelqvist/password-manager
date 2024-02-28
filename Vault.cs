using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace password_manager
{
    public class Vault
    {
        public static void ServerFileStructure(string filePath, string iv, string prop, string password)
        {
            List<VaultEntry> vaultEntries = new List<VaultEntry>();

            // Kontrollera om filen redan finns
            if (File.Exists(filePath))
            {
                // Läs innehållet från filen och deserialisera det till en lista av VaultEntry
                string json = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    vaultEntries = JsonSerializer.Deserialize<List<VaultEntry>>(json);
                }
            }

            // Lägg till nytt (prop, password)-par
            vaultEntries.Add(new VaultEntry { IV = iv, Prop = prop, Password = password });

            // Konvertera till JSON-sträng och spara i filen
            string jsonOutput = JsonSerializer.Serialize(vaultEntries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonOutput);

            Console.WriteLine("Lösenord tillagt i serverfilen.");
        }
    }

    // En klass för att representera varje inmatning i valvet
    public class VaultEntry
    {
        public string IV { get; set; }
        public string Prop { get; set; }
        public string Password { get; set; }
    }
}



























