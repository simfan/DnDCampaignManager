using BlazorApp1.DTOs;

namespace BlazorApp1.Services
{
    public class ResourceService
    {
        private readonly HttpClient _httpClient;

        public ResourceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<ResourceDto>> GetCampaignResourcesAsync(int campaignId)
        {
            return await _httpClient.GetFromJsonAsync<List<ResourceDto>>($"api/Resources/Campaign/{campaignId}");
        }

        public async Task<HttpResponseMessage> ShareResource(int resourceId, ShareResourceDto shareDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Resources/{resourceId}/share", shareDto);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
