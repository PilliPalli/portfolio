using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MeinPortfolio.Models;
using MeinPortfolio.Models.Commands;
using MeinPortfolio.Services;

namespace MeinPortfolio.Components
{
    public partial class Terminal
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private CommandService CommandService { get; set; }
        [Inject] private NavigationService NavigationService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

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
        protected override async Task OnInitializedAsync()
        {
            RegisterCommands();
            
            NavigationService.OnNavigate += HandleNavigate;
            LanguageService.OnLanguageChanged += OnLanguageChanged;

            _output.Add(new OutputLine("Welcome to the interactive Terminal Portfolio of Moritz Kreis. Type 'help' for available commands."));
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initTerminal");
                await FocusInputAsync();
            }
        }

        private void RegisterCommands()
        {
            CommandService.RegisterCommand(new HelpCommand(CommandService));

            CommandService.RegisterCommand(new CdCommand(NavigationService));
            CommandService.RegisterCommand(new PwdCommand(NavigationService));
            CommandService.RegisterCommand(new LsCommand(NavigationService));
            CommandService.RegisterCommand(new CatCommand(NavigationService));
            CommandService.RegisterCommand(new HomeCommand(NavigationService));

            CommandService.RegisterCommand(new ClearCommand());
            CommandService.RegisterCommand(new DateCommand());
            CommandService.RegisterCommand(new WhoamiCommand());


            CommandService.RegisterCommand(new CvCommand(JSRuntime));
            
            CommandService.RegisterCommand(new FunFactCommand(LanguageService));
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
                return commandList.Where(c => c.StartsWith(token, StringComparison.OrdinalIgnoreCase))
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

            // Bereits im Cycle? → nur vorwärts (+1)
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

            // gemeinsamer Präfix → erstmal bis dahin erweitern
            var common = CommonPrefix(candidates);
            if (!string.IsNullOrEmpty(common) && common.Length > token.Length)
            {
                ResetCycle();
                var completed = fixedPrefix + common;

                // Auto-Space bei vollständigem Befehl
                if (!currentInput.Contains(' ') && commandList.Contains(common))
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


        private List<string> commandList = new()
        {
            "help",
            "ls",
            "cat",
            "home",
            "clear",
            "whoami",
            "cd",
            "cv",
            "date",
            "pwd",
            "funfact"
        };
        
        private async Task ExecuteCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(_input)) return;

            var commandInput = _input.Trim();
            _output.Add(new OutputLine($"> {commandInput}", isCommand: true));

            var result = await CommandService.ExecuteCommandAsync(commandInput);

            if (result.Output == "clear")
            {
                _output.Clear();
                _output.Add(new OutputLine("Welcome to the interactive Terminal Portfolio of Moritz Kreis. Type 'help' for available commands."));
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
                    {
                        _output.Add(new OutputLine(line));
                    }
                }
            }

            _input = "";
            await FocusInputAsync();
            StateHasChanged();

            var isReady = await JSRuntime.InvokeAsync<bool>("eval", "window.terminalReady === true");

            if (isReady)
            {
                await JSRuntime.InvokeVoidAsync("scrollTerminalToBottom");
            }

        }

        private async Task FocusInputAsync()
        {
            await JSRuntime.InvokeVoidAsync("eval", "document.querySelector('.terminal-input input').focus()");
        }
        

        private void HandleNavigate(NavigationSection section)
        {
            StateHasChanged();
        }
        
        public void Dispose()
        {
            NavigationService.OnNavigate -= HandleNavigate;
            LanguageService.OnLanguageChanged -= OnLanguageChanged;
        }
        
        private void OnLanguageChanged(LanguageType language)
        {
            StateHasChanged();
        }
    }
}
