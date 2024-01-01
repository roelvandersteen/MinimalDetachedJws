# MinimalDetachedJws

A handler to create and verify Detached JSON Web Signatures in Compact Notation.

## Overview

This class provides functionality to create and verify Detached JSON Web Signatures (JWS) in Compact Notation. It supports HMAC SHA-256 signatures and follows the guidelines outlined in [RFC 7515 - JSON Web Signature (JWS)](https://datatracker.ietf.org/doc/html/rfc7515). The class is designed to be simple and minimalistic, focusing on the essential features of creating and verifying a Detached JWS.

## Usage

### Installation

You can install the `MinimalDetachedJws` package via NuGet Package Manager Console:

```pwsh
Install-Package MinimalDetachedJws
```

Or using the .NET CLI:

```pwsh
dotnet add package MinimalDetachedJws
```

### Example

```csharp
using MinimalDetachedJws;

// Initialize DetachedJwsHandler with a secret key
var handler = new DetachedJwsHandler("YourSecretKey");

// Create a Detached JWS
var payload = new Payload { Key = "value" };
string detachedSignature = handler.CreateDetachedJws(payload);

// Verify the Detached JWS
bool isSignatureValid = handler.VerifyDetachedJws(payload, detachedSignature);
```

## Constructors

### `DetachedJwsHandler(byte[] secretKeyBytes)`

Constructs a `DetachedJwsHandler` with the specified secret key provided as a byte array.

### `DetachedJwsHandler(string secretKeyString)`

Constructs a `DetachedJwsHandler` with the specified secret key provided as a string.

## Methods

### `CreateDetachedJws<T>(T payload)`

Creates a Detached JWS for the given payload.

### `VerifyDetachedJws<T>(T payload, string detachedSignature)`

Verifies the Detached JWS against the provided payload.

## Remarks

- The class only supports HMAC SHA-256 signatures.
- Ensure that the secret key is kept confidential and is known to both the sender and receiver.

## Usage in Azure Environment with Azure Key Vault

To enhance security, it's recommended to store sensitive information such as secret keys in a secure vault. In an Azure environment, Azure Key Vault provides a secure and convenient solution for managing secrets. Below is an example of how to integrate the `DetachedJwsHandler` with Azure Key Vault.

### Prerequisites

1. **Azure Key Vault Setup:** Create an Azure Key Vault and add a secret containing your JWS secret key.

2. **Service Principal:** Ensure that your application has the necessary permissions to access the Azure Key Vault. You may create a service principal with the required permissions, or use Managed Identity.

### App Settings Configuration

In your application settings, reference the secret stored in Azure Key Vault.

```json
{
  "AzureKeyVault": {
    "VaultUri": "https://your-key-vault.vault.azure.net/",
    "SecretName": "YourJwsSecretKeySecretName"
  }
}
```

### Code Example

```csharp
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using MinimalDetachedJws;

// Load configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Retrieve secret from Azure Key Vault
var azureServiceTokenProvider = new AzureServiceTokenProvider();
var keyVaultClient = new KeyVaultClient(
    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)
);

var vaultUri = configuration["AzureKeyVault:VaultUri"];
var secretName = configuration["AzureKeyVault:SecretName"];

var secretBundle = await keyVaultClient.GetSecretAsync(vaultUri, secretName);

// Use the secret key in DetachedJwsHandler
var handler = new DetachedJwsHandler(secretBundle.Value);

// Now you can use the DetachedJwsHandler as before
var payload = new Payload { Key = "value" };
string detachedSignature = handler.CreateDetachedJws(payload);
bool isSignatureValid = handler.VerifyDetachedJws(payload, detachedSignature);
```

Ensure that your application running in an Azure environment has the necessary permissions to access the Key Vault. You may configure these permissions through Azure RBAC (Role-Based Access Control) or Azure Managed Identity.

## Licence

Copyright 2024 Roel van der Steen

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.