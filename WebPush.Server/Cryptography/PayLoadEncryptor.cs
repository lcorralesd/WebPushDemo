namespace WebPush.Server.Cryptography;
internal class PayLoadEncryptor
{
    // This method is called from WebPush.Server\Builders\WebPushHttpRequestBuilder.cs
    public static WebPushBodyBuilderResult Encrypt(string subscriptionP256DH,
        string subscriptionAuth, string payload)
    {

        var p256DHBytes = subscriptionP256DH.ToBytesFromBase64();
        var authBytes = subscriptionAuth.ToBytesFromBase64();
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        var salt = GenerateSalt(16);

        var localKeysCurve = new ECDiffieHellmanP256();

        var sharedSecret = localKeysCurve.GetSharedSecret(p256DHBytes);

        var prk = DoHKDF(authBytes, sharedSecret, Encoding.UTF8.GetBytes("Content-Encoding: auth\0"), 32);

        var serverPublicKey = localKeysCurve.PublicKey;

        var context = GetContext(p256DHBytes, serverPublicKey);

        var nonceInfo = CreateContentEncoding("nonce", context);
        var contextEncryptionKeyInfo = CreateContentEncoding("aesgcm", context);

        var nonce = DoHKDF(salt, prk, nonceInfo, 12);
        var contentEncryptionKey = DoHKDF(salt, prk, contextEncryptionKeyInfo, 16);

        var input = AddPaddingToInput(payloadBytes);

        var encriptedPayload = AESGCM.Encrypt(nonce, contentEncryptionKey, input);

        return new WebPushBodyBuilderResult
        {
            Salt = salt,
            Payload = encriptedPayload,
            PublicKey = serverPublicKey
        };

    }

    static byte[] GenerateSalt(int length)
    {
        var salt = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        rng.GetNonZeroBytes(salt);
        return salt;
    }

    public static byte[] DoHKDF(byte[] salt, byte[] ikm, byte[] info, int length) =>
        HKDF.DeriveKey(HashAlgorithmName.SHA256, ikm,length, salt, info);

    public static byte[] ConvertInt(int number)
    {
        var outPut = BitConverter.GetBytes(Convert.ToUInt16(number));
        if(BitConverter.IsLittleEndian)
        {
            Array.Reverse(outPut);
        }
        return outPut;
    }

    public static byte[] GetContext(byte[] subscriptionP256dh, byte[] localPublicKey)
    {
        var context = new List<byte>();
        context.AddRange(Encoding.UTF8.GetBytes("P-256\0"));
        context.AddRange(ConvertInt(subscriptionP256dh.Length));
        context.AddRange(subscriptionP256dh);

        context.AddRange(ConvertInt(localPublicKey.Length));
        context.AddRange(localPublicKey);

        return context.ToArray();
    }

    public static byte[] CreateContentEncoding(string contentEncoding, byte[] context)
    {
        var contentEncodingBytes = Encoding.UTF8.GetBytes($"Content-Encoding: {contentEncoding}\0");

        var info = new List<byte>();
        info.AddRange(contentEncodingBytes);
        info.AddRange(context);

        return info.ToArray();
    }

    static byte[] AddPaddingToInput(byte[] data)
    {
        var input = new byte[data.Length + 2];
        Buffer.BlockCopy(data, 0, input, 2, data.Length);
        return input;
    }
}
