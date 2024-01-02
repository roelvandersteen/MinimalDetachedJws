namespace MinimalDetachedJws;

/// <summary>
///     A handler to create and verify Detached JSON Web Signatures in Compact Notation. This class cannot be
///     inherited.
/// </summary>
/// <see href="https://datatracker.ietf.org/doc/html/rfc7515#appendix-F">JSON Web Signature (JWS) - Detached Content</see>
public sealed class DetachedJwsHandler
{
    private const string SigningAlgorithm = "HS256";
    private readonly byte[] _secretKey;

    /// <summary>Constructor.</summary>
    /// <param name="secretKeyBytes">The secret key as a byte array.</param>
    public DetachedJwsHandler(byte[] secretKeyBytes)
    {
        _secretKey = secretKeyBytes;
    }

    /// <summary>Constructor.</summary>
    /// <param name="secretKeyString">The secret key as a string.</param>
    public DetachedJwsHandler(string secretKeyString)
    {
        _secretKey = Encoding.UTF8.GetBytes(secretKeyString);
    }

    /// <summary>Creates a Detached JWS.</summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="payload">The payload.</param>
    /// <returns>The new Detached JWS.</returns>
    /// <remarks>We only support HMAC SHA-256 signatures.</remarks>
    public string CreateDetachedJws<T>(T payload)
    {
        var header = Base64UrlEncoder.Encode(JsonSerializer.Serialize(new JwsHeader(SigningAlgorithm)));
        var signature = Base64UrlEncoder.Encode(CreateSignatureBytes(payload));
        return $"{header}..{signature}";
    }

    /// <summary>Verify the Detached JWS against the payload.</summary>
    /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
    /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
    /// <typeparam name="T">The payload types.</typeparam>
    /// <param name="payload">The payload.</param>
    /// <param name="detachedSignature">The Detached JWS.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    /// <remarks>We only support HMAC SHA-256 signatures.</remarks>
    public bool VerifyDetachedJws<T>(T payload, string detachedSignature)
    {
        var parts = detachedSignature.Split('.');
        if (parts.Length != 3)
        {
            throw new ArgumentException("JWS Compact Serialization string must consist of exactly three parts", nameof(detachedSignature));
        }

        if (!IsJwsHeaderValid(parts[0]))
        {
            throw new NotSupportedException("Invalid header: we only support HMAC SHA-256 as the signing algorithm");
        }

        var computedSignature = CreateSignatureBytes(payload);
        var signatureBytes = Base64UrlEncoder.DecodeBytes(parts[2]);

        // Compare the computed signature with the provided signature
        return CryptographicOperations.FixedTimeEquals(signatureBytes, computedSignature);
    }

    private byte[] CreateSignatureBytes<T>(T payload)
    {
        using var hmac = new HMACSHA256(_secretKey);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
    }

    private static bool IsJwsHeaderValid(string encodedHeader)
    {
        var jwsHeader = JsonSerializer.Deserialize<JwsHeader>(Base64UrlEncoder.Decode(encodedHeader));
        return jwsHeader?.Algorithm == SigningAlgorithm;
    }

    private sealed record JwsHeader([property: JsonPropertyName("alg")] string Algorithm);
}