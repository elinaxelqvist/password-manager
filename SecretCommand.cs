using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace password_manager
{
    public class Command 

    {

        public static string SecretCommand(string[] input)
        {
            if (File.Exists(input[1]))
            {
                string json = File.ReadAllText(input[1]);

                // Deserialisera JSON-strängen till en Dictionary<string, string>
                Dictionary<string, string> clientData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                // Kontrollera om deserialiseringen var framgångsrik och om klientData innehåller nyckeln "SecretKey"
                if (clientData != null && clientData.ContainsKey("SecretKey"))
                {
                    // Hämta värdet som är associerat med nyckeln "SecretKey" och skriv ut det
                    string secretKey = clientData["SecretKey"];

                    return secretKey;
                }
                else
                {
                    Console.WriteLine("Kunde inte hitta SecretKey i JSON-data eller JSON-data var ogiltig.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Filen existerar inte.");

                return null;
            }
        }

    }



}
          
            
       

   
