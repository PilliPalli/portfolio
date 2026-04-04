using MeinPortfolio.Services;
using MeinPortfolio.Models;
using MeinPortfolio.Models.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;

namespace MeinPortfolio.Components;

public partial class SectionIntro : ComponentBase, IDisposable
{
    [Inject] private IHttpClientFactory HttpFactory { get; set; } = default!;

    private enum Panel { Terminal, CodeGen }
    private Panel ActivePanel { get; set; } = Panel.Terminal;

    private List<OutputLine> _output = new();
    private string _input = "";
    private ElementReference _inputElement;

    private List<string> _cycleCandidates = new();
    private int _cycleIndex = -1;
    private string _cycleFixedPrefix = "";
    private string _inputBeforeCycle = "";

    private static readonly List<string> Sections = new()
    {
        "about", "projects", "contact", "home", "~"
    };

    // AI CodeGen fields
    private string _prompt = "";
    private string _selectedLanguage = "C#";
    private bool _isGenerating = false;
    private string _statusMessage = "";
    private bool _isError = false;

    protected override async Task OnInitializedAsync()
    {
        RegisterCommands();

        NavigationService.OnNavigate += HandleNavigate;
        LanguageService.OnLanguageChanged += OnLanguageChanged;

        _output.Add(new OutputLine("Welcome to my interactive Terminal Portfolio. Type 'help' for available commands."));
    }

    private async Task SwitchPanelAsync(Panel panel)
    {
        ActivePanel = panel;
        StateHasChanged();
        if (panel == Panel.Terminal)
        {
            await InvokeAsync(async () =>
            {
                await Task.Yield();
                await FocusInputAsync();
                await JSRuntime.InvokeVoidAsync("bindTerminalInput");
            });
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initTerminal");
            await FocusInputAsync();
            await JSRuntime.InvokeVoidAsync("bindTerminalInput");
        }
    }

    private void RegisterCommands()
    {
        CommandService.RegisterCommand(new HelpCommand(CommandService));

        CommandService.RegisterCommand(new CdCommand(NavigationService));
        CommandService.RegisterCommand(new PwdCommand(NavigationService));
        CommandService.RegisterCommand(new LsCommand(NavigationService));
        CommandService.RegisterCommand(new CatCommand(NavigationService, LanguageService));
        CommandService.RegisterCommand(new HomeCommand(NavigationService));

        CommandService.RegisterCommand(new ClearCommand());
        CommandService.RegisterCommand(new DateCommand());
        CommandService.RegisterCommand(new WhoamiCommand());

        CommandService.RegisterCommand(new CvCommand(JSRuntime, LanguageService));

        CommandService.RegisterCommand(new FunFactCommand(LanguageService));

        // AI: CodeGen command nur wenn Feature aktiviert
        if (Features.AiEnabled)
        {
            CommandService.RegisterCommand(new CodeGenCommand(HttpFactory, JSRuntime));
        }
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "Enter":
                ResetCycle();
                await ExecuteCommandAsync();
                break;

            case "ArrowUp":
                ResetCycle();
                _input = CommandService.GetPreviousCommand();
                StateHasChanged();
                break;

            case "ArrowDown":
                ResetCycle();
                _input = CommandService.GetNextCommand();
                StateHasChanged();
                break;

            case "Tab":
                _input = GetAutoCompleteSuggestion(_input);
                StateHasChanged();
                await FocusInputAsync();
                break;

            default:
                ResetCycle();
                break;
        }
    }

    private void ResetCycle()
    {
        _cycleCandidates.Clear();
        _cycleIndex = -1;
        _cycleFixedPrefix = "";
        _inputBeforeCycle = "";
    }

    private (string fixedPrefix, string token) SplitAtLastToken(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return ("", "");

        int lastSpace = text.LastIndexOf(' ');
        if (lastSpace < 0)
            return ("", text);

        var fixedPrefix = text.Substring(0, lastSpace + 1);
        var token = text.Substring(lastSpace + 1);
        return (fixedPrefix, token);
    }

    private string CommonPrefix(IEnumerable<string> items)
    {
        var list = items.ToList();
        if (!list.Any()) return "";

        string prefix = list[0];
        foreach (var s in list.Skip(1))
        {
            int i = 0;
            int max = Math.Min(prefix.Length, s.Length);
            while (i < max && prefix[i] == s[i]) i++;
            prefix = prefix.Substring(0, i);
            if (prefix.Length == 0) break;
        }
        return prefix;
    }

    private IEnumerable<string> GetCandidates(string fullInput, string token)
    {
        if (!fullInput.Contains(' '))
        {
            return CommandList.Where(c => c.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x);
        }

        var firstSpace = fullInput.IndexOf(' ');
        var cmd = fullInput.Substring(0, firstSpace).Trim().ToLower();

        switch (cmd)
        {
            case "cd":
                return Sections.Where(s => s.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x);

            case "cat":
                var files = VirtualFileSystem.GetFiles(NavigationService.CurrentSection).Keys;
                return files.Where(f => f.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x);

            default:
                return Enumerable.Empty<string>();
        }
    }

    private string GetAutoCompleteSuggestion(string currentInput)
    {
        if (string.IsNullOrWhiteSpace(currentInput))
            return currentInput;

        if (_cycleCandidates.Count > 0 && currentInput == BuildCurrentCycleInput())
        {
            if (_cycleCandidates.Count == 1)
                return currentInput;

            _cycleIndex = (_cycleIndex + 1) % _cycleCandidates.Count;
            return _cycleFixedPrefix + _cycleCandidates[_cycleIndex];
        }

        var (fixedPrefix, token) = SplitAtLastToken(currentInput);
        var candidates = GetCandidates(currentInput, token).ToList();
        if (candidates.Count == 0)
        {
            ResetCycle();
            return currentInput;
        }

        var common = CommonPrefix(candidates);
        if (!string.IsNullOrEmpty(common) && common.Length > token.Length)
        {
            ResetCycle();
            var completed = fixedPrefix + common;

            if (!currentInput.Contains(' ') && CommandList.Contains(common))
                completed += " ";

            return completed;
        }

        _cycleCandidates = candidates;
        _cycleIndex = 0;
        _cycleFixedPrefix = fixedPrefix;
        _inputBeforeCycle = currentInput;

        return _cycleFixedPrefix + _cycleCandidates[_cycleIndex];
    }

    private string BuildCurrentCycleInput()
    {
        if (_cycleCandidates.Count == 0 || _cycleIndex < 0) return _inputBeforeCycle;
        return _cycleFixedPrefix + _cycleCandidates[_cycleIndex];
    }

    // Command list: dynamisch je nach Feature Toggle
    private List<string> CommandList
    {
        get
        {
            var list = new List<string>
            {
                "help", "ls", "cat", "home", "clear", "whoami", "cd", "cv", "date", "pwd", "funfact"
            };
            if (Features.AiEnabled)
            {
                list.Add("codegen");
            }
            return list;
        }
    }

    private async Task ExecuteCommandAsync()
    {
        if (string.IsNullOrWhiteSpace(_input)) return;

        var commandInput = _input.Trim();
        _output.Add(new OutputLine($"> {commandInput}", isCommand: true));

        var result = await CommandService.ExecuteCommandAsync(commandInput);

        if (result.Output == "clear")
        {
            _output.Clear();
            _output.Add(new OutputLine("Welcome to my interactive Terminal Portfolio. Type 'help' for available commands."));
            _input = "";
            StateHasChanged();
            return;
        }

        if (!string.IsNullOrEmpty(result.Output))
        {
            if (result.IsHtml)
            {
                _output.Add(new OutputLine(result.Output, isHtml: true));
            }
            else
            {
                foreach (var line in result.Output.Split('\n'))
                    _output.Add(new OutputLine(line));
            }
        }

        _input = "";
        await FocusInputAsync();
        StateHasChanged();

        var isReady = await JSRuntime.InvokeAsync<bool>("eval", "window.terminalReady === true");
        if (isReady)
            await JSRuntime.InvokeVoidAsync("scrollTerminalToBottom");
    }

    private async Task FocusInputAsync()
    {
        await JSRuntime.InvokeVoidAsync("eval", "document.querySelector('.terminal-input input').focus()");
    }

    private void HandleNavigate(NavigationSection section)
    {
        StateHasChanged();
    }

    // AI: Code-Generator im Panel
    private async Task GenerateCode()
    {
        if (string.IsNullOrWhiteSpace(_prompt))
        {
            _statusMessage = LanguageService.GetText("Bitte gib einen Prompt ein.", "Please enter a prompt.");
            _isError = true;
            return;
        }

        if (_prompt.Trim().Length < 10)
        {
            _statusMessage = LanguageService.GetText(
                "Der Prompt muss mindestens 10 Zeichen lang sein.",
                "Prompt must be at least 10 characters long.");
            _isError = true;
            return;
        }

        _isGenerating = true;
        _statusMessage = "";
        _isError = false;
        StateHasChanged();

        try
        {
            var request = new
            {
                prompt = _prompt,
                language = _selectedLanguage
            };

            var api = HttpFactory.CreateClient("CodeGenApi");
            using var resp = await api.PostAsJsonAsync("/api/generate", request);

            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _statusMessage = LanguageService.GetText("Rate-Limit erreicht. Bitte 15 Sekunden warten.", "Rate limit reached. Please wait 15 seconds.");
                }
                else
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    _statusMessage = LanguageService.GetText($"Fehler: {err}", $"Error: {err}");
                }
                _isError = true;
                return;
            }

            var zipBytes = await resp.Content.ReadAsByteArrayAsync();
            var fileName = $"generated-code-{DateTime.Now:yyyyMMdd-HHmmss}.zip";
            await JSRuntime.InvokeVoidAsync("downloadZip", zipBytes, fileName);

            _statusMessage = LanguageService.GetText(
                $"Code erfolgreich generiert! Download von {fileName} gestartet...",
                $"Code generated successfully! Download of {fileName} started...");
            _isError = false;
        }
        catch (HttpRequestException)
        {
            _statusMessage = LanguageService.GetText(
                "Fehler: Code-Generator-Service nicht erreichbar.",
                "Error: Code generator service unavailable.");
            _isError = true;
        }
        catch (Exception ex)
        {
            _statusMessage = LanguageService.GetText(
                $"Unerwarteter Fehler: {ex.Message}",
                $"Unexpected error: {ex.Message}");
            _isError = true;
        }
        finally
        {
            _isGenerating = false;
            _prompt = "";
            StateHasChanged();
        }
    }

    private void OnLanguageChanged(LanguageType language)
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        NavigationService.OnNavigate -= HandleNavigate;
        LanguageService.OnLanguageChanged -= OnLanguageChanged;
    }
}
