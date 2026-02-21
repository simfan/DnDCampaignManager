using BlazorApp1.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

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

        public async Task<HttpResponseMessage> UploadResourceAsync(string name, string description, int campaignId, IBrowserFile file)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream(52428800));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);
            content.Add(new StringContent(name), "Name");
            content.Add(new StringContent(description), "Description");
            content.Add(new StringContent(campaignId.ToString()), "CampaignId");
            return await _httpClient.PostAsync("api/Resources/upload", content);
        }

        public async Task<HttpResponseMessage> ShareResource(int resourceId, ShareResourceDto shareDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Resources/{resourceId}/share", shareDto);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
