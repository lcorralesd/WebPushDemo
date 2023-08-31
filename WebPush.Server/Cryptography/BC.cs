using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace WebPush.Server.Cryptography;
internal class BC
{
    public static byte[] ComputeSecret(byte[] bobPrivateKey, byte[] alicePublicKey)
    {
        BigInteger D = new BigInteger(1, bobPrivateKey);
        Org.BouncyCastle.Math.EC.ECPoint publicPoint = GetPublicPointFromPublicKeyBytes(alicePublicKey);

        Org.BouncyCastle.Math.EC.ECPoint otherPointInTheCurve = publicPoint.Multiply(D).Normalize();

        return otherPointInTheCurve.AffineXCoord.ToBigInteger().ToByteArrayUnsigned();

    }

    static Org.BouncyCastle.Math.EC.ECPoint GetPublicPointFromPublicKeyBytes(byte[] publicKey)
    {
        X9ECParameters curveParameters = ECNamedCurveTable.GetByName("secp256r1");
        ECDomainParameters domainParameters = new ECDomainParameters(curveParameters.Curve, curveParameters.G, curveParameters.N, curveParameters.H);

        return domainParameters.Curve.CreatePoint(
            new BigInteger(1, publicKey[1..33]),
            new BigInteger(1, publicKey[33..]));
    }
}
