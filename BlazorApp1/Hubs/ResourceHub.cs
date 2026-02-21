using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp1.Hubs
{

    public class ResourceHub : Hub
    {
        /// <summary>
        /// Called by clients when they join the Player Hub page so they receive
        /// resource-shared events for every campaign they belong to.
        /// </summary>
        public async Task JoinCampaign(string campaignId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"campaign_{campaignId}");
        }

        public async Task LeaveCampaign(string campaignId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"campaign_{campaignId}");
        }
    }
}