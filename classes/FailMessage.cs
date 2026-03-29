public class FailMessage
{
    public string? Log { get; set; }
    public string GameMessage { get; set; }

    public FailMessage(string log, string? gameMessage = null)
    {
        Log = log;
        if (gameMessage != null)
            GameMessage = gameMessage;
        else
            GameMessage = log;
    }
}
