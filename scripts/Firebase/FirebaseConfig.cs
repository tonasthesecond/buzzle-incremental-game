public static class FirebaseConfig
{
    public const string ApiKey = "YOUR_FIREBASE_API_KEY";
    public const string ProjectId = "YOUR_PROJECT_ID";
    public const string AuthBase = "https://identitytoolkit.googleapis.com/v1/accounts";
    public const string TokenBase = "https://securetoken.googleapis.com/v1/token";
    public const string StoreBase =
        $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";
}
