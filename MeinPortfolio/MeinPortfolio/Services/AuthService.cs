using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.Extensions.Options;
using MeinPortfolio.Models;

namespace MeinPortfolio.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly PortfolioConfig _config;
        private const string AUTH_STORAGE_KEY = "portfolio_auth";

        public bool IsAuthenticated { get; private set; } = false;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, IOptions<PortfolioConfig> config)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _config = config.Value;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var storedAuth = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AUTH_STORAGE_KEY);
                IsAuthenticated = !string.IsNullOrEmpty(storedAuth) && storedAuth == "authenticated";
            }
            catch
            {
                IsAuthenticated = false;
            }
        }

        public async Task<bool> LoginAsync(string password)
        {
            if (password == _config.Password)
            {
                IsAuthenticated = true;
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AUTH_STORAGE_KEY, "authenticated");
                return true;
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            IsAuthenticated = false;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AUTH_STORAGE_KEY);
        }

    }
}