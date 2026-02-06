using BlazorApp1.DTOs;
using System.Net.Http.Json;

namespace BlazorApp1.Services
{
    public class CampaignService
    {
        private readonly HttpClient _httpClient;

        public CampaignService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CampaignDto>> GetCampaignsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CampaignDto>>("api/campaigns") ?? new List<CampaignDto>();
        }

        public async Task<CampaignDetailDto?> GetCampaignAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CampaignDetailDto>($"api/campaigns/{id}");
        }

        public async Task<CampaignDto?> CreateCampaignAsync(CreateCampaignDto campaign)
        {
            var response = await _httpClient.PostAsJsonAsync("api/campaigns", campaign);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CampaignDto>();
        }

        public async Task UpdateCampaignAsync(int id, UpdateCampaignDto campaign)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/campaigns/{id}", campaign);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCampaignAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/campaigns/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}