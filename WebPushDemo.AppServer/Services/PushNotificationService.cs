using Microsoft.Extensions.Options;
using System.Text.Json;
using WebPush.Server.Models;
using WebPush.Server.Services;
using WebPushDemo.AppServer.Options;
using WebPushDemo.Shared.Entities;

namespace WebPushDemo.AppServer.Services;

public class PushNotificationService
{
    readonly IServiceScopeFactory _serviceScopeFactory;
    readonly VapidInfoOptions _vapidInfoOptions;
    readonly string _dataFileName;

    public PushNotificationService(IWebHostEnvironment environment, IServiceScopeFactory serviceScopeFactory, IOptions<VapidInfoOptions> vapidInfoOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _vapidInfoOptions = vapidInfoOptions.Value;
        _dataFileName = Path.Combine(environment.ContentRootPath, "Data", "Data.json");
    }

    public async Task Subscribe(WebPushSubscription subscription)
    {
        using var fs = new FileStream(_dataFileName, FileMode.Create);
        await JsonSerializer.SerializeAsync(fs, subscription);
    }

    public void SendExampleNotification()
    {
        Task.Run(async () =>
            {
                var delay = 45000;
                using var scope = _serviceScopeFactory.CreateScope();
                var webPushService = scope.ServiceProvider.GetRequiredService<WebPushService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<PushNotificationService>>();

                var stream = new FileStream(_dataFileName, FileMode.Open, FileAccess.Read);

                var subscriptionData =
                    await JsonSerializer.DeserializeAsync<WebPushSubscription>(stream);

                var subscription = new SubscriptionInfo(subscriptionData.Endpoint,
                    subscriptionData.P256dh, subscriptionData.Auth);
                stream.Dispose();

                var vapidInfo = new VapidInfo(_vapidInfoOptions.Subject, _vapidInfoOptions.PublicKey, _vapidInfoOptions.PrivateKey);

                await SendNotificationAsync(webPushService, logger, subscription, vapidInfo, "When i grow up to be a watermelon!");
                await Task.Delay(delay);

                await SendNotificationAsync(webPushService, logger, subscription, vapidInfo, "When i grow up to be an apple!");
                await Task.Delay(delay);

                await SendNotificationAsync(webPushService, logger, subscription, vapidInfo, "When i grow up to be a pineapple!");
                await Task.Delay(delay);

            });
    }

    static async Task SendNotificationAsync(WebPushService webPushService,
        ILogger<PushNotificationService> logger, SubscriptionInfo subscription, VapidInfo vapidInfo, string message)
    {
        var payLoad = new
        {
            message = message,
            url = "/counter"
        };

        var serializedPayload = JsonSerializer.Serialize(payLoad);
        try
        {
            await webPushService.SendNotificationAsync(subscription, serializedPayload, vapidInfo);
            logger.LogInformation("*** Send '{0}' notification", message);
        }
        catch (Exception ex)
        {
            logger.LogError("{0}", ex.Message);
        }
    }
}
