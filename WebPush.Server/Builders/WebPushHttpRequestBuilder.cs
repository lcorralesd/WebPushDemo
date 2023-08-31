using System.Net.Http.Headers;

namespace WebPush.Server.Builders;
internal class WebPushHttpRequestBuilder
{
    public static HttpRequestMessage Build(SubscriptionInfo subscriptionInfo, string payLoad, VapidInfo vapidInfo)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, subscriptionInfo.Endpoint);

        var webPushAuthorizacionToken = WebPushRequestTokenBuilder.Build(subscriptionInfo, vapidInfo);

        var encryptedPayLoad = PayLoadEncryptor.Encrypt(subscriptionInfo.P256DH, subscriptionInfo.Auth, payLoad);

        request.Headers.Add("Authorization", $"WebPush {webPushAuthorizacionToken}");
        request.Headers.Add("Encryption", $"salt={encryptedPayLoad.Salt.ToBase64UrlString()}");
        request.Headers.Add("Crypto-Key", string.Format("dh={0};p256ecdsa={1}",
            encryptedPayLoad.PublicKey.ToBase64UrlString(), vapidInfo.PublicKey));

        const int timeToLiveInSeconds = 604800;
        request.Headers.Add("TTL", timeToLiveInSeconds.ToString());

        request.Content = new ByteArrayContent(encryptedPayLoad.Payload);
        request.Content.Headers.ContentLength = encryptedPayLoad.Payload.Length;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        request.Content.Headers.ContentEncoding.Add("aesgcm");

        return request;
    }
}
