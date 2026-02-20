using BlazorApp1.DTOs;
using System.Net.Http.Json;

namespace BlazorApp1.Services
{
    public class InventoryService
    {
        private readonly HttpClient _http;

        public InventoryService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<InventoryItemDto>> GetCharacterInventoryAsync(int characterId)
        {
            return await _http.GetFromJsonAsync<List<InventoryItemDto>>($"api/Inventory/character/{characterId}")
                   ?? new List<InventoryItemDto>();
        }

        public async Task<List<InventoryItemDto>> GetCampaignInventoryAsync(int campaignId)
        {
            return await _http.GetFromJsonAsync<List<InventoryItemDto>>($"api/Inventory/campaign/{campaignId}")
                   ?? new List<InventoryItemDto>();
        }

        public async Task<InventoryItemDto?> CreateItemAsync(CreateInventoryItemDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/Inventory", dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<InventoryItemDto>();
            return null;
        }

        public async Task<InventoryItemDto?> UpdateItemAsync(int id, UpdateInventoryItemDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/Inventory/item/{id}", dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<InventoryItemDto>();
            return null;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Inventory/item/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<InventoryItemDto?> TransferItemAsync(TransferInventoryItemDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/Inventory/transfer", dto);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<InventoryItemDto>();
            return null;
        }
    }
}