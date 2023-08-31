namespace WebPush.Server.Builders;
internal class WebPushRequestTokenBuilder
{
    public static string Build(SubscriptionInfo subscriptionInfo, VapidInfo vapidInfo)
    {
        var jwtHeader = new Dictionary<string, object>
        {
            { "typ", "JWT" },
            { "alg", "ES256" },
        };

        var uri = new Uri(subscriptionInfo.Endpoint);
        var audience = $"{uri.Scheme}://{uri.Host}";

        var jwtPayload = new Dictionary<string, object>
        {
            { "aud", audience },
            { "exp", DateTimeOffset.UtcNow.AddHours(12).ToUnixTimeSeconds() },
            { "sub", vapidInfo.Subject },
        };

        // encode en base64
        var jwtHeaderEncoded = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(jwtHeader)).ToBase64UrlString();

        var jwtPayloadEncoded = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(jwtPayload)).ToBase64UrlString();

        // signature
        var unsignedToken = $"{jwtHeaderEncoded}.{jwtPayloadEncoded}";

        var signature = ECDSA.Sign(unsignedToken,
            vapidInfo.PrivateKey.ToBytesFromBase64Url()).ToBase64UrlString();

        return $"{unsignedToken}.{signature}";
    }
}
