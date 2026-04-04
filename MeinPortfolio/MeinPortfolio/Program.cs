using MeinPortfolio.Services;
using MeinPortfolio;
using MeinPortfolio.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace MeinPortfolio;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // Feature Flags
        var features = new FeatureFlags();
        builder.Configuration.GetSection("Features").Bind(features);
        builder.Services.AddSingleton(features);

        // IHttpClientFactory immer registrieren (wird von SectionIntro injiziert)
        builder.Services.AddHttpClient();

        // CodeGen API HttpClient nur registrieren wenn AI aktiviert ist
        if (features.AiEnabled)
        {
            string apiBaseUrl;
            try
            {
                using var cfgClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
                var cfg = await cfgClient.GetFromJsonAsync<Dictionary<string, string>>("config.json");
                apiBaseUrl = cfg?["ApiBaseUrl"] ?? "https://meinportfolio-codegenapi.onrender.com";
            }
            catch
            {
                apiBaseUrl = "https://meinportfolio-codegenapi.onrender.com";
            }

            builder.Services.AddHttpClient("CodeGenApi", c =>
            {
                c.BaseAddress = new Uri(apiBaseUrl);
            });
        }

        builder.Services.AddSingleton<CommandService>();
        builder.Services.AddScoped<LanguageService>();
        builder.Services.AddSingleton<NavigationService>();
        builder.Services.Configure<PortfolioConfig>(
            builder.Configuration.GetSection("Portfolio"));

        await builder.Build().RunAsync();
    }
}
