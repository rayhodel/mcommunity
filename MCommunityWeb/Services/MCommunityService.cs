using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MCommunityWeb.Models;

namespace MCommunityWeb.Services
{
    public class MCommunityService
    {
        private readonly HttpClient _httpClient;

        public MCommunityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://mcommunity.umich.edu/api/");
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            var request = new TokenRequest { Username = username, Password = password };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("token/", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Authentication failed: {response.StatusCode} - {errorContent}");
            }

            try
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return tokenResponse?.AccessToken ?? throw new Exception("Invalid token response");
            }
            catch (JsonException ex)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to parse token response. Content: {responseContent}", ex);
            }
        }

        public async Task<PersonResponse?> GetPersonAsync(string token, string uniqname)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"people/{uniqname}/");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PersonResponse>();
        }

        public async Task<GroupResponse?> GetGroupAsync(string token, string groupName)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"groups/{groupName}/");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GroupResponse>();
        }
    }
}
