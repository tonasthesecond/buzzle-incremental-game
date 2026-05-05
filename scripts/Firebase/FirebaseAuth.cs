using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public partial class FirebaseAuth : Node
{
    [Signal]
    public delegate void LoginSucceededEventHandler(string userId);

    [Signal]
    public delegate void LoginFailedEventHandler(string errorMessage);

    [Signal]
    public delegate void RegisterSucceededEventHandler(string userId);

    [Signal]
    public delegate void RegisterFailedEventHandler(string errorMessage);

    [Signal]
    public delegate void TokenRefreshedEventHandler();

    [Signal]
    public delegate void LoggedOutEventHandler();

    public string UserId { get; private set; } = "";
    public string IdToken { get; private set; } = "";
    public string RefreshToken { get; private set; } = "";
    public DateTimeOffset? IdTokenIssuedAtUtc { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(IdToken);

    private const string AuthCachePath = "user://auth_cache.json";

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public override void _Ready()
    {
        LoadRefreshTokenFromCache();
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        try
        {
            var response = await SendAuthRequestAsync(
                $"{FirebaseConfig.AuthBase}:signUp?key={FirebaseConfig.ApiKey}",
                new AuthRequest(email, password, true)
            );

            ApplyAuthState(response);
            EmitSignal(SignalName.RegisterSucceeded, UserId);
            EmitSignal(SignalName.LoginSucceeded, UserId);
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[FirebaseAuth] Register failed: {ex}");
            EmitSignal(SignalName.RegisterFailed, ex.Message);
            return false;
        }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var response = await SendAuthRequestAsync(
                $"{FirebaseConfig.AuthBase}:signInWithPassword?key={FirebaseConfig.ApiKey}",
                new AuthRequest(email, password, true)
            );

            ApplyAuthState(response);
            EmitSignal(SignalName.LoginSucceeded, UserId);
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[FirebaseAuth] Login failed: {ex}");
            EmitSignal(SignalName.LoginFailed, ex.Message);
            return false;
        }
    }

    public async Task<bool> RefreshIdTokenIfNeededAsync()
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
            return false;

        if (IdTokenIssuedAtUtc.HasValue && DateTimeOffset.UtcNow - IdTokenIssuedAtUtc.Value < TimeSpan.FromMinutes(55))
            return true;

        return await RefreshIdTokenAsync();
    }

    public async Task<bool> RefreshIdTokenAsync()
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
            return false;

        try
        {
            var response = await SendTokenRefreshRequestAsync();

            IdToken = response.IdToken ?? "";
            RefreshToken = response.RefreshToken ?? RefreshToken;
            UserId = response.UserId ?? UserId;
            IdTokenIssuedAtUtc = DateTimeOffset.UtcNow;

            SaveRefreshTokenToCache();
            EmitSignal(SignalName.TokenRefreshed);
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[FirebaseAuth] Token refresh failed: {ex}");
            return false;
        }
    }

    public void Logout()
    {
        UserId = "";
        IdToken = "";
        RefreshToken = "";
        IdTokenIssuedAtUtc = null;

        if (FileAccess.FileExists(AuthCachePath))
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(AuthCachePath));

        EmitSignal(SignalName.LoggedOut);
    }

    private async Task<AuthSuccessResponse> SendAuthRequestAsync(string url, AuthRequest payload)
    {
        var body = JsonSerializer.Serialize(payload, JsonOpts);
        var headers = new string[] { "Content-Type: application/json" };
        var responseBody = await SendHttpRequestAsync(url, headers, HttpClient.Method.Post, body);

        var response = JsonSerializer.Deserialize<AuthSuccessResponse>(responseBody, JsonOpts);
        if (response == null || string.IsNullOrWhiteSpace(response.LocalId) || string.IsNullOrWhiteSpace(response.IdToken))
            throw new InvalidOperationException("Firebase auth response was incomplete.");

        return response;
    }

    private async Task<TokenRefreshResponse> SendTokenRefreshRequestAsync()
    {
        var formBody =
            $"grant_type=refresh_token&refresh_token={Uri.EscapeDataString(RefreshToken)}";
        var headers = new string[] { "Content-Type: application/x-www-form-urlencoded" };
        var url = $"{FirebaseConfig.TokenBase}?key={FirebaseConfig.ApiKey}";
        var responseBody = await SendHttpRequestAsync(url, headers, HttpClient.Method.Post, formBody);

        var response = JsonSerializer.Deserialize<TokenRefreshResponse>(responseBody, JsonOpts);
        if (response == null || string.IsNullOrWhiteSpace(response.IdToken))
            throw new InvalidOperationException("Firebase token refresh response was incomplete.");

        return response;
    }

    private async Task<string> SendHttpRequestAsync(
        string url,
        string[] headers,
        HttpClient.Method method,
        string body
    )
    {
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

        if (statusCode < 200 || statusCode >= 300)
        {
            GD.PrintErr($"[FirebaseAuth] HTTP {statusCode}: {responseBody}");
            throw new InvalidOperationException(ExtractFirebaseErrorMessage(responseBody));
        }

        return responseBody;
    }

    private void ApplyAuthState(AuthSuccessResponse response)
    {
        UserId = response.LocalId ?? "";
        IdToken = response.IdToken ?? "";
        RefreshToken = response.RefreshToken ?? "";
        IdTokenIssuedAtUtc = DateTimeOffset.UtcNow;
        SaveRefreshTokenToCache();
    }

    private void LoadRefreshTokenFromCache()
    {
        if (!FileAccess.FileExists(AuthCachePath))
            return;

        try
        {
            var json = FileAccess.GetFileAsString(AuthCachePath);
            var cache = JsonSerializer.Deserialize<AuthCacheData>(json, JsonOpts);
            if (cache != null && !string.IsNullOrWhiteSpace(cache.RefreshToken))
                RefreshToken = cache.RefreshToken;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[FirebaseAuth] Failed to load auth cache: {ex}");
        }
    }

    private void SaveRefreshTokenToCache()
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
            return;

        try
        {
            using var file = FileAccess.Open(AuthCachePath, FileAccess.ModeFlags.Write);
            file.StoreString(JsonSerializer.Serialize(new AuthCacheData(RefreshToken), JsonOpts));
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[FirebaseAuth] Failed to save auth cache: {ex}");
        }
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

    private sealed record AuthRequest(string Email, string Password, bool ReturnSecureToken);

    private sealed record AuthCacheData(string RefreshToken);

    private sealed class AuthSuccessResponse
    {
        [JsonPropertyName("localId")]
        public string? LocalId { get; set; }

        [JsonPropertyName("idToken")]
        public string? IdToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }
    }

    private sealed class TokenRefreshResponse
    {
        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
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
