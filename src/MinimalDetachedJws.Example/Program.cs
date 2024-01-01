var payload = new SamplePayload { Key = "value" };

var detachedJwsHandler = new DetachedJwsHandler("YourSecretKey");

// Create detached JWS
var detachedSignature = detachedJwsHandler.CreateDetachedJws(payload);
Console.WriteLine(detachedSignature);

// Verify detached JWS
var isSignatureValid = detachedJwsHandler.VerifyDetachedJws(payload, detachedSignature);
Console.WriteLine($"Signature is valid: {isSignatureValid}");