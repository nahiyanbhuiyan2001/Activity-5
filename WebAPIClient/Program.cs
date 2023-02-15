using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

//I tried my very best, I was having a few issues scanning the api databsae for the sunrise times

namespace SunriseSunsetApiExample
{
    class SunriseSunsetResponse
    {
        // Maps the "sunrise" JSON property to this C# property, and applies a custom JSON converter to parse the value as a DateTime
        [JsonPropertyName("sunrise")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Sunrise { get; set; }

        // Maps the "sunset" JSON property to this C# property, and applies a custom JSON converter to parse the value as a DateTime
        [JsonPropertyName("sunset")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Sunset { get; set; }
    }

    class DateTimeConverter : JsonConverter<DateTime>
{
    // Overrides the default behavior for reading a DateTime from JSON
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Check if the JSON value is a string that can be parsed as a DateTime, and return the parsed value
        if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out DateTime dateTime))
        {
            return dateTime;
        }

        // Otherwise, throw an exception to indicate the JSON value is invalid
        throw new JsonException("Invalid date/time format.");
    }

    // Overrides the default behavior for writing a DateTime to JSON
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Writes the DateTime value as an ISO 8601 string (e.g. "2022-02-15T16:30:00.0000000-08:00")
        writer.WriteStringValue(value.ToString("o"));
    }
}
    class Program
    {
        static async Task Main(string[] args)
        {
            // Prompt the user to enter a latitude
            Console.Write("Enter latitude: ");
            // Read the user's input and store it in a variable
            string latitude = Console.ReadLine();

            // Prompt the user to enter a longitude
            Console.Write("Enter longitude: ");
            // Read the user's input and store it in a variable
            string longitude = Console.ReadLine();

            // Construct the URL for the Sunrise Sunset API using the latitude and longitude entered by the user
            string apiUrl = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}";

            // Create an instance of HttpClient, which will be used to make an HTTP request to the API
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Send an HTTP GET request to the API URL and wait for the response
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    // Check that the response was successful (i.e. has a 2xx status code)
                    response.EnsureSuccessStatusCode();

                    // Read the response body as a string
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Output the response body to the console
                    Console.WriteLine(responseBody);

                    // Deserialize the response JSON into a SunriseSunsetResponse object
                    SunriseSunsetResponse apiResponse = JsonSerializer.Deserialize<SunriseSunsetResponse>(responseBody);

                    // Output the sunrise and sunset times to the console
                    Console.WriteLine($"Sunrise: {apiResponse.Sunrise}");
                    Console.WriteLine($"Sunset: {apiResponse.Sunset}");
                }
                catch (HttpRequestException e)
                {
                    // Handle any errors that occurred during the HTTP request
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }
    }

}
