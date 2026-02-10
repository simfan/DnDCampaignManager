using BlazorApp1.DTOs;
using BlazorApp1.Models;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace BlazorApp1.Services
{
    public class JournalService
    {
        private readonly HttpClient _httpClient;

        public JournalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JournalDto>> GetJournalsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<JournalDto>>("api/Journals") ?? new List<JournalDto>();
        }

        public async Task<JournalDetailDto?> GetJournalAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<JournalDetailDto>($"api/Journals/{id}");
        }

        public async Task<JournalDto?> CreateJournalAsync(CreateJournalDto journal)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Journals", journal);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<JournalDto>();
        }

        public async Task UpdateJournalAsync(int id, UpdateJournalDto journal)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Journals/{id}", journal);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteJournalAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Journals/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<EntryDetailDto> GetJournalEntry(int id)
        {
            return await _httpClient.GetFromJsonAsync<EntryDetailDto>($"api/Entries/{id}");

        }

        public async Task<EntryDto> CreateJournalEntry(CreateEntryDto entry)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Entries", entry);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EntryDto>();
        }
    }
}
