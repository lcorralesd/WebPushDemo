namespace WebPush.Server.Cryptography;
internal static class ECDSA
{
    public static byte[] Sign(string valueToSign, byte[] privateKey)
    {
        using ECDsa ecdsa = ECDsa.Create();
        ECParameters eCParameters = new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            D = privateKey,
        };

        ecdsa.ImportParameters(eCParameters);

        var signature =
            ecdsa.SignData(Encoding.UTF8.GetBytes(valueToSign), HashAlgorithmName.SHA256);

        return signature;

    }
}
