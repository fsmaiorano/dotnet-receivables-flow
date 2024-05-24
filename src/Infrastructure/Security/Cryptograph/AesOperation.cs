using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

public class AesOperation
{
    public async Task<string> EncryptAsync(string clearText, string passphrase)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(passphrase);
        aes.IV = _iv;
        using MemoryStream output = new();
        await using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
        await cryptoStream.FlushFinalBlockAsync();
        return Convert.ToBase64String(output.ToArray());
    }

    public async Task<string> DecryptAsync(string encryptedBase64, string passphrase)
    {
        byte[] encrypted;
        try
        {
            encrypted = Convert.FromBase64String(encryptedBase64);
        }
        catch (FormatException e)
        {
            Console.WriteLine($"Failed to convert encrypted text '{encryptedBase64}' to byte array: {e.Message}");
            throw;
        }

        using var aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(passphrase);
        aes.IV = _iv;
        using MemoryStream input = new(encrypted);
        await using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using MemoryStream output = new();
        await cryptoStream.CopyToAsync(output);
        return Encoding.Unicode.GetString(output.ToArray());
    }

    private static byte[] DeriveKeyFromPassword(string password)
    {
        var emptySalt = Array.Empty<byte>();
        const int iterations = 1000;
        const int desiredKeyLength = 16; // 16 bytes equal 128 bits.
        var hashMethod = HashAlgorithmName.SHA384;
        return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
            emptySalt,
            iterations,
            hashMethod,
            desiredKeyLength);
    }

    private readonly byte[] _iv =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };
}