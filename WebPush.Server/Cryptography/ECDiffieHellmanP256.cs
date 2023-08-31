namespace WebPush.Server.Cryptography;
internal class ECDiffieHellmanP256
{
    public ECDiffieHellman EcDiffieHellman { get; }
    public byte[] PublicKey { get; }
    public byte[] PrivateKey { get; }

    public ECDiffieHellmanP256()
    {
        ECDiffieHellman Ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

        PublicKey = GetPublicKey(Ecdh);
        PrivateKey = GetPrivateKey(Ecdh);

        EcDiffieHellman = Ecdh;
    }

    static byte[] GetPrivateKey(ECDiffieHellman ecdh)
    {
        ECParameters ECParam = ecdh.ExportParameters(true);
        return ECParam.D;
    }

    static byte[] GetPublicKey(ECDiffieHellman ecdh)
    {
        ECParameters ECParam = ecdh.PublicKey.ExportParameters();
        byte[] XBytes = ECParam.Q.X;
        byte[] YBytes = ECParam.Q.Y;

        var rawPublicKeyBytes = new List<byte>
        {
            4
        };

        rawPublicKeyBytes.AddRange(XBytes);
        rawPublicKeyBytes.AddRange(YBytes);

        return rawPublicKeyBytes.ToArray();
    }

    public byte[] GetSharedSecret(byte[] alicePublicKeyBytes) =>
        BC.ComputeSecret(PrivateKey, alicePublicKeyBytes);

}
