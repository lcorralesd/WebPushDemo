namespace WebPush.Server.Cryptography;
internal class AESGCM
{
    public static byte[] Encrypt(byte[] nonce, byte[] key, byte[] message)
    {
        using AesGcm aesGcm = new AesGcm(key);
        byte[] cipherText = new byte[message.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        aesGcm.Encrypt(nonce, message, cipherText, tag);

        byte[] result = new byte[cipherText.Length + tag.Length];
        Buffer.BlockCopy(cipherText, 0, result, 0, cipherText.Length);
        Buffer.BlockCopy(tag, 0, result, cipherText.Length, tag.Length);
        return result;
    }
}
