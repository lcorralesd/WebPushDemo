using Microsoft.AspNetCore.Components;
using WebPush.Blazor.Models;
using WebPush.Blazor.Services;
using WebPushDemo.Client.Services;
using WebPushDemo.Shared.Entities;

namespace WebPushDemo.Client.Pages;

public partial class SubscriptionPage
{

    [Inject]
    WebPushService PushNotificationService { get; set; }

    [Inject]
    PushNotificationServerService PushNotificationServerService { get; set; }

    SubscriptionInfo SubscriptionInfo;
    public string Message;

    async Task GetSubscriptionAsync()
    {
        Message = string.Empty;
        SubscriptionInfo = await PushNotificationService.GetSubscriptionAsync();
        if (SubscriptionInfo == default) 
        {
            Message = "No subscription found.";
        }
    }

    async Task SendSubscriptionAsync()
    {
        var success = await PushNotificationServerService.SendNotificationAsync(
            new WebPushSubscription
            {
                Endpoint = SubscriptionInfo.Endpoint,
                P256dh = SubscriptionInfo.P256dh,
                Auth = SubscriptionInfo.Auth
            });

        if (success)
        {
            Message = "Datos enviados con exito";
        }
        else
        {
            Message = "Error al enviar los datos";
        }

    }

    async Task RequestExampleNotificationAsync()
    {
        var success = await PushNotificationServerService.RequestExampleNotification();
        if (success)
        {
            Message = "Solicitud de notificaccón enviada.";
        }
        else
        {
            Message = "Error de solicitud de notificacion";
        }
    }
}
