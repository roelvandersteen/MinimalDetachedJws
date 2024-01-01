namespace MinimalDetachedJws.Tests;

public class DetachedJwsHandlerTests
{
    public sealed class TestPayload
    {
        public string? Key { get; set; }
    }

    [Fact]
    public void CreateDetachedJws_ValidPayload_ReturnsDetachedSignature()
    {
        // Arrange
        var detachedJwsHandler = new DetachedJwsHandler("TestSecretKey");
        var payload = new TestPayload { Key = "random value" };

        // Act
        var detachedSignature = detachedJwsHandler.CreateDetachedJws(payload);

        // Assert
        Assert.NotNull(detachedSignature);
        Assert.Equal(3, detachedSignature.Split('.').Length);
    }

    [Fact]
    public void VerifyDetachedJws_InvalidDetachedSignatureFormat_ThrowsArgumentException()
    {
        // Arrange
        var detachedJwsHandler = new DetachedJwsHandler("TestSecretKey");
        const string invalidDetachedSignature = "invalidDetachedSignature";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => detachedJwsHandler.VerifyDetachedJws(new object(), invalidDetachedSignature));
    }

    [Fact]
    public void VerifyDetachedJws_InvalidJwsHeader_ThrowsNotSupportedException()
    {
        // Arrange
        var detachedJwsHandler = new DetachedJwsHandler("TestSecretKey");
        var payload = new TestPayload { Key = "random value" };
        var detachedSignatureParts = detachedJwsHandler.CreateDetachedJws(payload).Split('.');
        detachedSignatureParts[0] = Base64UrlEncoder.Encode(JsonSerializer.Serialize(new { alg = "InvalidAlgorithm" }));
        var invalidDetachedSignature = string.Join('.', detachedSignatureParts);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => detachedJwsHandler.VerifyDetachedJws(payload, invalidDetachedSignature));
    }

    [Fact]
    public void VerifyDetachedJws_InvalidPayload_ReturnsFalse()
    {
        // Arrange
        var detachedJwsHandler = new DetachedJwsHandler("TestSecretKey");
        var payload = new TestPayload { Key = "random value" };
        var detachedSignature = detachedJwsHandler.CreateDetachedJws(payload);
        var invalidPayload = new TestPayload { Key = "differentValue" };

        // Act
        var isSignatureValid = detachedJwsHandler.VerifyDetachedJws(invalidPayload, detachedSignature);

        // Assert
        Assert.False(isSignatureValid);
    }

    [Fact]
    public void VerifyDetachedJws_InvalidSignature_ReturnsFalse()
    {
        // Arrange
        var detachedJwsHandler = new DetachedJwsHandler("TestSecretKey");
        var payload = new TestPayload { Key = "random value" };
        var detachedSignatureParts = detachedJwsHandler.CreateDetachedJws(payload).Split('.');
        detachedSignatureParts[2] = Base64UrlEncoder.Encode(Base64UrlEncoder.DecodeBytes(detachedSignatureParts[2]).Select(x => (byte)~x).ToArray());
        var invalidDetachedSignature = string.Join('.', detachedSignatureParts);

        // Act
        var isSignatureValid = detachedJwsHandler.VerifyDetachedJws(payload, invalidDetachedSignature);

        // Assert
        Assert.False(isSignatureValid);
    }

    [Fact]
    public void VerifyDetachedJws_ValidPayloadAndSignature_ReturnsTrue()
    {
        // Arrange
        var detachedJwsHandler = new DetachedJwsHandler("TestSecretKey");
        var payload = new TestPayload { Key = "random value" };
        var detachedSignature = detachedJwsHandler.CreateDetachedJws(payload);

        // Act
        var isSignatureValid = detachedJwsHandler.VerifyDetachedJws(payload, detachedSignature);

        // Assert
        Assert.True(isSignatureValid);
    }
}