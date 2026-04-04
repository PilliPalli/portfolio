using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using MeinPortfolio.Services;

namespace MeinPortfolio.Models.Commands;

public class CodeGenCommand : BaseCommand
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IJSRuntime _jsRuntime;
 
    public override string Name => "codegen";
    public override string Description => "Generate code from a prompt and download as ZIP";
    public override string Usage => "codegen <prompt> [--lang=C#]";

    public CodeGenCommand(IHttpClientFactory httpFactory, IJSRuntime jsRuntime)
    {
        _httpFactory = httpFactory;
        _jsRuntime = jsRuntime;
    }

    public override async Task<string> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            return "Usage: codegen <prompt> [--lang=C#]\n\nExamples:\n  codegen \"FizzBuzz in C#\"\n  codegen \"Tic-Tac-Toe game\" --lang=JavaScript";
        }

        var (prompt, options) = ParseArguments(args);
        prompt = prompt?.Trim();

        if (string.IsNullOrWhiteSpace(prompt))
            return "Error: Prompt is required";
        
        if (prompt.Length < 10)
            return "Error: Prompt must be at least 10 characters long.";

        try
        {
            var request = new {
                prompt,
                language = options.Language
            };

            var api = _httpFactory.CreateClient("CodeGenApi");
            using var resp = await api.PostAsJsonAsync("/api/generate", request);

            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    return "Rate limit reached. Please wait 15 seconds.";

                var err = await resp.Content.ReadAsStringAsync();
                return $"Error generating code: {(int)resp.StatusCode} – {err}";
            }

            var zipBytes = await resp.Content.ReadAsByteArrayAsync();
            var fileName = $"generated-code-{DateTime.Now:yyyyMMdd-HHmmss}.zip";
            await _jsRuntime.InvokeVoidAsync("downloadZip", zipBytes, fileName);

            return $"Code generated successfully! Download of {fileName} started...";
           
        }
        catch (HttpRequestException)
        {
            return "Error: Code generator service unavailable.";
        }
        catch (Exception ex)
        {
            return $"Unexpected error: {ex.Message}";
        }
    }


    private (string prompt, CodeGenOptions options) ParseArguments(string[] args)
    {
        var options = new CodeGenOptions();
        var promptParts = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            
            if (arg.StartsWith("--lang="))
            {
                options.Language = arg.Substring(7);
            }
            else
            {
                promptParts.Add(arg);
            }
        }

        return (string.Join(" ", promptParts), options);
    }

    private class CodeGenOptions
    {
        public string Language { get; set; } = "C#";
    }
}
