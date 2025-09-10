using Microsoft.JSInterop;
using System.Globalization;

namespace MeinPortfolio.Services
{
    public enum LanguageType
    {
        German,
        English
    }
    
    public class LanguageService
    {

        private readonly IJSRuntime _jsRuntime;
        private LanguageType _currentLanguage = LanguageType.German;

        public event Action<LanguageType>? OnLanguageChanged;

        public LanguageType CurrentLanguage                 // KURZ: public LanguageType CurrentLanguage => _currentLanguage;
        {
            get { return _currentLanguage; }
        }
        
        public LanguageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            var stored = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "portfolio_language");

            if (!string.IsNullOrEmpty(stored) && Enum.TryParse(stored, out LanguageType lang))
            {
                _currentLanguage = lang;
            }
            else
            {
                _currentLanguage = LanguageType.German;
            }
            
            await SetCulture(_currentLanguage);
            OnLanguageChanged?.Invoke(_currentLanguage);
        }

        public async Task SetLanguageAsync(LanguageType language)
        {
            if (_currentLanguage != language)
            {
                _currentLanguage = language;
                await SetCulture(language);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "portfolio_language", language.ToString());
                OnLanguageChanged?.Invoke(language);
            }
        }

        private Task SetCulture(LanguageType language)
        {
            string culture;
            if (language == LanguageType.German)
            {
                culture = "de-DE";
            }
            else
            {
                culture = "en-US";
            }

            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
            return Task.CompletedTask;
        }

        public string GetText(string germanText, string englishText)
        {
            string language;
            if (_currentLanguage == LanguageType.German)
            {
                language = germanText;
            }
            else
            {
                language = englishText;
            }

            return language;
        }
        
        public string GetCvPath()
        {
            if (_currentLanguage == LanguageType.German)
            {
                return "cv/CV_DE.pdf";
            }
            else
            {
                return "cv/CV_EN.pdf";
            }
        }
    }
    
}

