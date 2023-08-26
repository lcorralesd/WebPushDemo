using Microsoft.AspNetCore.Components;
using WebPush.Blazor.Models;
using WebPush.Blazor.Services;
using WebPushDemo.Client.Services;

namespace WebPushDemo.Client.Pages;

public partial class SubscriptionPage
{

    [Inject]
    WebPushService PushNotificationService { get; set; }

    [Inject]
    PushNotificationServerService PushNotificationServerService { get; set; }

    SubscriptionInfo SubscriptionInfo;
    string Message;

    async Task GetSubscriptionAsync()
    {
        Message = string.Empty;
        SubscriptionInfo = await PushNotificationService.GetSubscriptionAsync();
        if (SubscriptionInfo == default) 
        {
            Message = "No subscription found.";
        }
    }
}
