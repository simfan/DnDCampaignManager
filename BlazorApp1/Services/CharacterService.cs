using BlazorApp1.DTOs;
using System.Net.Http.Json;

namespace BlazorApp1.Services
{
    public class CharacterService
    {
        private readonly HttpClient _httpClient;

        public CharacterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CharacterDto>> GetCharactersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CharacterDto>>("api/characters") ?? new List<CharacterDto>();
        }

        public async Task<CharacterDto?> GetCharacterAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CharacterDto>($"api/characters/{id}");
        }

        public async Task<List<CharacterDto>> GetCharactersByCampaignAsync(int campaignId)
        {
            return await _httpClient.GetFromJsonAsync<List<CharacterDto>>($"api/characters/campaign/{campaignId}") ?? new List<CharacterDto>();
        }

        public async Task<CharacterDto?> CreateCharacterAsync(CreateCharacterDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("api/characters", character);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CharacterDto>();
        }

        public async Task UpdateCharacterAsync(int id, UpdateCharacterDto character)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/characters/{id}", character);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCharacterAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/characters/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<CharacterDto?> ImportFromDndBeyondAsync(DndBeyondImportRequest request)
        {
            if (request.DndBeyondCharacterId == null && string.IsNullOrWhiteSpace(request.RawJson))
                throw new Exception("No character ID or JSON provided.");
            var response = await _httpClient.PostAsJsonAsync("api/characters/import/dndbeyond", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"({(int)response.StatusCode}): {error}");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CharacterDto>();
        }
        /*public async Task<CreateCharacterDto> ImportFromDndBeyondAsync(long characterId, int campaignId)
        {
            var url = $"https://www.dndbeyond.com/character/{characterId}/json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("D&D Beyond returned an error. Make sure the character is set to public sharing.");

            var json = await response.Content.ReadAsStringAsync();
            return Map(json, campaignId);
        }*/
    }
}