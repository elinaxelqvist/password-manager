using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace password_manager
{
    public class SecretKey
    {

        //Metod som genererar en Secret Key
        public static string GenerateSecretKey()
        {
            // Skapa en byte-array för att lagra den hemliga nyckeln
            byte[] secretKey = new byte[32]; // 256 bitar //Är ett helt JSON objekt, med måsvingar etc. Det är ingen byte array. Vi vill bara åt värdet av secretKey.

            // Skapa och använd en instans av RandomNumberGenerator
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // Fyll byte-arrayen med slumpmässiga värden
                rng.GetBytes(secretKey);
            }

            string stringSecretKey = Convert.ToBase64String(secretKey); //Denna är redan en sträng, kan ej konverteras till en sträng.. Base64 är ingen kryptering.

            // Returnera den genererade nyckeln
            return stringSecretKey;
        }


        public static void SaveSecretKeyToFile(string filePath, string secretKey)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new StringJsonConverter() } // Använd vår anpassade konverterare
            };

            var userSecretKeys = new Dictionary<string, string>();
            userSecretKeys["SecretKey"] = secretKey;

            string updatedJson = JsonSerializer.Serialize(userSecretKeys, options);
            File.WriteAllText(filePath, updatedJson);
        }
    }

    public class StringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value ?? string.Empty); // Skriv ut strängen utan att använda escape-tecken
        }
    }
}

