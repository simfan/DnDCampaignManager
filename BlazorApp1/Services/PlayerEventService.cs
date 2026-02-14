using BlazorApp1.DTOs;
using System.Net.Http.Json;

namespace BlazorApp1.Services
{
    public class PlayerEventService
    {
        private readonly HttpClient _httpClient;

        public PlayerEventService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PlayerEventDto>> GetEventsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<PlayerEventDto>>("api/playerevents") ?? new List<PlayerEventDto>();
        }

        public async Task<List<PlayerEventDto>> GetEventsByMonthAsync(int year, int month)
        {
            return await _httpClient.GetFromJsonAsync<List<PlayerEventDto>>($"api/playerevents/month/{year}/{month}") ?? new List<PlayerEventDto>();
        }

        public async Task<PlayerEventDto?> GetEventAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<PlayerEventDto>($"api/playerevents/{id}");
        }

        public async Task<PlayerEventDto?> CreateEventAsync(CreatePlayerEventDto playerEvent)
        {
            var response = await _httpClient.PostAsJsonAsync("api/playerevents", playerEvent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PlayerEventDto>();
        }

        public async Task UpdateEventAsync(UpdatePlayerEventDto playerEvent)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/playerevents/{playerEvent.PlayerEventId}", playerEvent);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteEventAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/playerevents/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}