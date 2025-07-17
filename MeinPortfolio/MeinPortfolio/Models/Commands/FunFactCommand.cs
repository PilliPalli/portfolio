using System.Text.Json;
using MeinPortfolio.Services;

namespace MeinPortfolio.Models.Commands;

public class FunFactCommand : BaseCommand
{
    public override string Name => "funfact";
    public override string Description => "Display a random fun fact";
    public override string Usage => "funfact";

    static readonly HttpClient client = new();

    public override async Task<string> ExecuteAsync(string[] args)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(
                "https://uselessfacts.jsph.pl/api/v2/facts/random?language=en");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var factResponse = JsonSerializer.Deserialize<FunFactResponseService>(responseBody);

            if (factResponse.Text != null)
            {
                return factResponse.Text;
            }
            return "No fun fact available.";

        }
        catch (HttpRequestException)
        {
            return "An error occurred while retrieving the fun fact.";
        }
        catch (JsonException)
        {
            return "Error parsing the JSON format.";
        }
    }
}
