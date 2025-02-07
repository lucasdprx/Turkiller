using System.Collections.Generic;

public static class PlayerNameTracker
{
    private static Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();

    public static void AddPlayerName(ulong clientId, string name)
    {
        playerNames[clientId] = name;
    }

    public static bool TryGetPlayerName(ulong clientId, out string name)
    {
        return playerNames.TryGetValue(clientId, out name);
    }

    public static void RemovePlayerName(ulong clientId)
    {
        playerNames.Remove(clientId);
    }
}
