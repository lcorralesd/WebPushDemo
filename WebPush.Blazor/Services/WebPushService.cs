namespace WebPush.Blazor.Services;
public class WebPushService : IAsyncDisposable
{
    readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    readonly IOptions<WebPushNotificationOptions> _options;
    readonly ILogger<WebPushService> _logger;

    public WebPushService(IJSRuntime jsRuntime, IOptions<WebPushNotificationOptions> options,
        ILogger<WebPushService> logger)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                       "import", $"./_content/{GetType().Assembly.GetName().Name}/js/pushNotification.js")
        .AsTask());

        _options = options;

        _logger = logger;
    }

    public async Task<SubscriptionInfo> GetSubscriptionAsync()
    {
        SubscriptionInfo subscriptionInfo = default;
        try
        {
            var module = await _moduleTask.Value;
            subscriptionInfo = await module.InvokeAsync<SubscriptionInfo>("getSubscription",
                _options.Value.ServerPublicKey);
        }
        catch (Exception ex)
        {
            _logger.LogError("ERROR: {0}", ex.Message);
        }

        return subscriptionInfo;
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
