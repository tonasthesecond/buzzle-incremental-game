using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public partial class FirebaseSave : Node
{
    [Signal]
    public delegate void SaveSucceededEventHandler();

    [Signal]
    public delegate void SaveFailedEventHandler(string errorMessage);

    [Signal]
    public delegate void LoadSucceededEventHandler();

    [Signal]
    public delegate void LoadFailedEventHandler(string errorMessage);

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public async Task<bool> SaveAsync(FirebaseAuth auth, SaveData saveData)
    {
        try
        {
            await EnsureAuthenticatedAsync(auth);

            var document = new FirestoreDocument
            {
                Fields = new Dictionary<string, FirestoreValue>
                {
                    ["SaveJson"] = new FirestoreValue
                    {
                        StringValue = JsonSerializer.Serialize(saveData, JsonOpts),
                    },
                    ["UpdatedAt"] = new FirestoreValue
                    {
                        StringValue = DateTimeOffset.UtcNow.ToString("O"),
                    },
                },
            };

            var url = $"{FirebaseConfig.StoreBase}/saves/{auth.UserId}";
            var headers = new string[]
            {
                "Content-Type: application/json",
                $"Authorization: Bearer {auth.IdToken}",
            };

            await SendWithRetryAsync(
                auth,
                url,
                headers,
                HttpClient.Method.Patch,
                JsonSerializer.Serialize(document, JsonOpts)
            );

            EmitSignal(SignalName.SaveSucceeded);
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[FirebaseSave] Save failed: {ex}");
            EmitSignal(SignalName.SaveFailed, ex.Message);
            return false;
        }
    }

    public async Task<SaveData?> LoadAsync(FirebaseAuth auth)
    {
        try
        {
            await EnsureAuthenticatedAsync(auth);

            var url = $"{FirebaseConfig.StoreBase}/saves/{auth.UserId}";
            var headers = new string[] { $"Authorization: Bearer {auth.IdToken}" };
            var responseBody = await SendWithRetryAsync(auth, url, headers, HttpClient.Method.Get, "");

            var document = JsonSerializer.Deserialize<FirestoreDocumentResponse>(responseBody, JsonOpts);
            var saveJson = document?.Fields?.GetValueOrDefault("SaveJson")?.StringValue;
            if (string.IsNullOrWhiteSpace(saveJson))
                throw new InvalidOperationException("SaveJson was missing from Firestore document.");

            var saveData = JsonSerializer.Deserialize<SaveData>(saveJson, JsonOpts) ?? new SaveData();
            saveData.UnlockedKeys = new List<string>(new HashSet<string>(saveData.UnlockedKeys));

            EmitSignal(SignalName.LoadSucceeded);
            return saveData;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[FirebaseSave] Load failed: {ex}");
            EmitSignal(SignalName.LoadFailed, ex.Message);
            return null;
        }
    }

    private async Task EnsureAuthenticatedAsync(FirebaseAuth auth)
    {
        if (!await auth.RefreshIdTokenIfNeededAsync())
            throw new InvalidOperationException("User is not authenticated.");
    }

    private async Task<string> SendWithRetryAsync(
        FirebaseAuth auth,
        string url,
        string[] headers,
        HttpClient.Method method,
        string body
    )
    {
        var attempts = 0;

        while (true)
        {
            attempts++;

            var http = new HttpRequest();
            AddChild(http);
            http.Timeout = 10.0;

            var error = http.Request(url, headers, method, body);
            if (error != Error.Ok)
            {
                http.QueueFree();
                throw new InvalidOperationException($"Failed to start HTTP request: {error}");
            }

            var result = await ToSignal(http, HttpRequest.SignalName.RequestCompleted);
            var statusCode = (long)result[1];
            var responseBytes = (byte[])result[3];
            var responseBody = Encoding.UTF8.GetString(responseBytes);
            http.QueueFree();

            if (statusCode >= 200 && statusCode < 300)
                return responseBody;

            GD.PrintErr($"[FirebaseSave] HTTP {statusCode}: {responseBody}");

            if (statusCode == 401 && attempts == 1 && await auth.RefreshIdTokenAsync())
            {
                headers = ReplaceAuthHeader(headers, auth.IdToken);
                continue;
            }

            if (statusCode == 404 && method == HttpClient.Method.Get)
                throw new InvalidOperationException("Remote save not found.");

            if (statusCode == 403)
            {
                auth.Logout();
                throw new InvalidOperationException("Access forbidden. User has been logged out.");
            }

            if (statusCode == 429 && attempts < 3)
            {
                await ToSignal(GetTree().CreateTimer(5.0), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            if (statusCode >= 500)
                throw new InvalidOperationException("Firebase server error.");

            throw new InvalidOperationException(ExtractFirebaseErrorMessage(responseBody));
        }
    }

    private static string[] ReplaceAuthHeader(string[] headers, string idToken)
    {
        var updated = new string[headers.Length];

        for (var i = 0; i < headers.Length; i++)
        {
            updated[i] = headers[i].StartsWith("Authorization:", StringComparison.OrdinalIgnoreCase)
                ? $"Authorization: Bearer {idToken}"
                : headers[i];
        }

        return updated;
    }

    private static string ExtractFirebaseErrorMessage(string responseBody)
    {
        try
        {
            var response = JsonSerializer.Deserialize<FirebaseErrorResponse>(responseBody, JsonOpts);
            return response?.Error?.Message ?? $"Firebase request failed: {responseBody}";
        }
        catch
        {
            return $"Firebase request failed: {responseBody}";
        }
    }

    private sealed class FirestoreDocument
    {
        [JsonPropertyName("fields")]
        public Dictionary<string, FirestoreValue> Fields { get; set; } = new();
    }

    private sealed class FirestoreDocumentResponse
    {
        [JsonPropertyName("fields")]
        public Dictionary<string, FirestoreValue>? Fields { get; set; }
    }

    private sealed class FirestoreValue
    {
        [JsonPropertyName("stringValue")]
        public string? StringValue { get; set; }
    }

    private sealed class FirebaseErrorResponse
    {
        [JsonPropertyName("error")]
        public FirebaseErrorBody? Error { get; set; }
    }

    private sealed class FirebaseErrorBody
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
