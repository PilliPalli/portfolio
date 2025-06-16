using Microsoft.JSInterop;
using Microsoft.Extensions.Options;
using MeinPortfolio.Models;

namespace MeinPortfolio.Services
{
    public class AuthService
    {
        private readonly IJSRuntime _js;
        private readonly PortfolioConfig _cfg;
        private const string AUTH_KEY = "portfolio_auth";

        public bool IsAuthenticated { get; private set; }

        public AuthService(IJSRuntime js, IOptions<PortfolioConfig> cfg)
        {
            _js  = js;
            _cfg = cfg.Value;
        }

        public async Task InitializeAsync()
        {
            var stored = await _js.InvokeAsync<string>("localStorage.getItem", AUTH_KEY);
            IsAuthenticated = stored == "authenticated";
        }

        public async Task<bool> LoginAsync(string password)
        {
            if (password != _cfg.Password) return false;

            IsAuthenticated = true;
            await _js.InvokeVoidAsync("localStorage.setItem", AUTH_KEY, "authenticated");


            
            return true;
        }

        public async Task LogoutAsync()
        {
            IsAuthenticated = false;
            await _js.InvokeVoidAsync("localStorage.removeItem", AUTH_KEY);
        }
    }
}