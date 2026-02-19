using BlazorApp1.DTOs;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
namespace BlazorApp1.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;

        public ChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ChatRoomDto>>GetChatRoomsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ChatRoomDto>>("api/Chat/Rooms");
        }

        public async Task<List<ChatMessageDto>> GetChatMessagesAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<List<ChatMessageDto>>($"api/Chat/rooms/{id}/messages");
        }

        public async Task<ChatRoomDto?> CreateChatRoomAsync(CreateChatRoomDto chatRoom)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Chat/Rooms", chatRoom);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ChatRoomDto>();
        }

        public async Task<HttpResponseMessage> SendMessageAsync(SendMessageDto message)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Chat/Messages", message);
            
            return response;
        }
        public async Task MarkAsReadAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/Chat/Rooms/{id}/mark-read", null);
            response.EnsureSuccessStatusCode();
   
        }
        public async Task<List<dynamic>> GetCampaignMembersAsync(int campaignId)
        {
            return await _httpClient.GetFromJsonAsync<List<dynamic>>($"api/Chat/campaigns/{campaignId}/members");
        }
    }
}
