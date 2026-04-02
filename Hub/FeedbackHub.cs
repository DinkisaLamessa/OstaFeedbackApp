using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace OstaFeedbackApp.Hubs
{
    public class FeedbackHub : Hub
    {
        // Optional: send new feedback to all connected clients
        public async Task SendFeedbackUpdate(object feedback)
        {
            await Clients.All.SendAsync("ReceiveFeedback", feedback);
        }
    }
}