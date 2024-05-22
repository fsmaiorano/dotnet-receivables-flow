using Infrastructure.Security;
using static NUnit.Framework.Assert;

namespace UnitTests.Infrastructure.Security;

[TestFixture]
public class AesOperationUnitTest
{
    private AesOperation _aesOperation;
    private const string Passphrase = "testPassphrase";

    [SetUp]
    public void Setup()
    {
        _aesOperation = new AesOperation();
    }

    [Test]
    public async Task EncryptAsync_ShouldReturnEncryptedString_WhenGivenPlainText()
    {
        var plainText = "Hello, World!";
        var encryptedText = await _aesOperation.EncryptAsync(plainText, Passphrase);

        That(encryptedText, Is.Not.EqualTo(plainText));
        IsFalse(string.IsNullOrWhiteSpace(encryptedText));
    }

    [Test]
    public async Task DecryptAsync_ShouldReturnDecryptedString_WhenGivenEncryptedText()
    {
        var plainText = "Hello, World!";
        var encryptedText = await _aesOperation.EncryptAsync(plainText, Passphrase);
        var decryptedText = await _aesOperation.DecryptAsync(encryptedText, Passphrase);
        That(decryptedText, Is.EqualTo(plainText));
    }

    [Test]
    public void DecryptAsync_ShouldThrowException_WhenGivenInvalidEncryptedText()
    {
        var invalidEncryptedText = "InvalidEncryptedText";
        ThrowsAsync<System.FormatException>(async () =>
            await _aesOperation.DecryptAsync(invalidEncryptedText, Passphrase));
    }

    [Test]
    public async Task EncryptAsync_ShouldReturnDifferentResults_WhenGivenSamePlainTextWithDifferentPassphrases()
    {
        var plainText = "Hello, World!";
        var passphrase1 = "passphrase1";
        var passphrase2 = "passphrase2";

        var encryptedText1 = await _aesOperation.EncryptAsync(plainText, passphrase1);
        var encryptedText2 = await _aesOperation.EncryptAsync(plainText, passphrase2);
        That(encryptedText2, Is.Not.EqualTo(encryptedText1));
    }
}