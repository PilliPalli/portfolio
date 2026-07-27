using System.Threading.Tasks;
using Microsoft.JSInterop;
using MeinPortfolio.Services;

namespace MeinPortfolio.Models.Commands
{
    public class CvCommand : BaseCommand
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly LanguageService _languageService;

        public override string Name => "cv";
        public override string Description => "Download CV as PDF";
        public override string Usage => "cv";

        public CvCommand(IJSRuntime jsRuntime, LanguageService languageService)
        {
            _jsRuntime = jsRuntime;
            _languageService = languageService;
        }

        public override async Task<string> ExecuteAsync(string[] args)
        {
            var url = _languageService.GetCvPath();
            var name = Path.GetFileName(url);

            await _jsRuntime.InvokeVoidAsync("downloadCv", url, name);
            return _languageService.GetText("Downloading CV…", "Downloading CV…");
        }
    }
}
