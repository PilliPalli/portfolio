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
        private LanguageType _currentLanguage = LanguageType.English;

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
            var storedLanguage = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "portfolio_language");

            if (string.IsNullOrEmpty(storedLanguage) && Enum.TryParse<LanguageType>(storedLanguage, out var language))
            {
                _currentLanguage = language;
            }
            else
            {
                _currentLanguage = LanguageType.English;
            }

            await SetCultureAsync(_currentLanguage);
            OnLanguageChanged?.Invoke(_currentLanguage);
        }

        public async Task SetLanguageAsync(LanguageType language)
        {
            if (_currentLanguage != language)
            {
                _currentLanguage = language;
                await SetCultureAsync(language);
                await _jsRuntime.InvokeVoidAsync("localStorage.getItem", "portfolio_language", language.ToString());
                OnLanguageChanged?.Invoke(language);
            }
        }

        private async Task SetCultureAsync(LanguageType language)
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
    }
    
}

