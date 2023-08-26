using System.Net.Http.Json;
using WebPushDemo.Shared.Entities;

namespace WebPushDemo.Client.Services;

public class PushNotificationServerService
{
    private readonly HttpClient _httpClient;

    public PushNotificationServerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendNotificationAsync(WebPushSubscription webPushSubscription)
    {
        var response = await _httpClient.PostAsJsonAsync("subscribe", webPushSubscription);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RequestExampleNotification()
    {
        var response = await _httpClient.GetAsync("request-example-notification");
        return response.IsSuccessStatusCode;
    }
}
