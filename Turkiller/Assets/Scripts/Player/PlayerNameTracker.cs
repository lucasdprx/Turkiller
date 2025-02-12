using System.Collections.Generic;

public static class PlayerNameTracker
{
    private static Dictionary<ulong, (string name, int skinIndex)> playerData = new Dictionary<ulong, (string, int)>();


    public static void AddPlayerData(ulong clientId, string name, int skinIndex)
    {
        playerData[clientId] = (name, skinIndex);
    }

    public static bool TryGetPlayerData(ulong clientId, out string name, out int skinIndex)
    {
        if (playerData.TryGetValue(clientId, out var data))
        {
            name = data.name;
            skinIndex = data.skinIndex;
            return true;
        }
        name = "Joueur";
        skinIndex = 0;
        return false;
    }

    public static void RemovePlayerData(ulong clientId)
    {
        playerData.Remove(clientId);
    }
}
