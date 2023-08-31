namespace WebPush.Server.Services;
public class WebPushService
{
    readonly HttpClient _httpClient;

    public WebPushService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendNotificationAsync(SubscriptionInfo subscriptionInfo,
        string payLoad, VapidInfo vapidInfo, CancellationToken cancellationToken = default)
    {
        var request = WebPushHttpRequestBuilder.Build(subscriptionInfo, payLoad, vapidInfo);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if(!response.IsSuccessStatusCode)
        {
            await HandleResponseError(response);
        }
    }

    static async Task HandleResponseError(HttpResponseMessage response)
    {
        var responseCodeMessage =
            $"Recived unexpected response code: {response.StatusCode}";

        switch(response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                responseCodeMessage = "Bad Request";
                break;
            case HttpStatusCode.RequestEntityTooLarge:
                responseCodeMessage = "Payload too large";
                break;
            
            case HttpStatusCode.TooManyRequests:
                responseCodeMessage = "Too many requests";
                break;
            case HttpStatusCode.NotFound:
            case HttpStatusCode.Gone:
                responseCodeMessage = "Subscription no longer valid";
                break;
        }

        string details = null;
        if (response.Content != null)
        {
            details = await response.Content.ReadAsStringAsync();
        }

        var message = string.IsNullOrEmpty(details)
            ? responseCodeMessage
            : $"{responseCodeMessage}. Details {details}";

        throw new Exception(message);
    }
}
