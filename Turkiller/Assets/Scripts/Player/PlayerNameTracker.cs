using System.Collections.Generic;

public static class PlayerNameTracker
{
    private static Dictionary<ulong, (string name, int skinIndex, int voiceIndex)> playerData = new Dictionary<ulong, (string, int, int)>();


    public static void AddPlayerData(ulong clientId, string name, int skinIndex, int voiceIndex)
    {
        playerData[clientId] = (name, skinIndex, voiceIndex);
    }

    public static bool TryGetPlayerData(ulong clientId, out string name, out int skinIndex, out int voiceIndex)
    {
        if (playerData.TryGetValue(clientId, out var data))
        {
            name = data.name;
            skinIndex = data.skinIndex;
            voiceIndex = data.voiceIndex;
            return true;
        }
        name = "Joueur";
        skinIndex = 0;
        voiceIndex = 0;
        return false;
    }

    public static void RemovePlayerData(ulong clientId)
    {
        playerData.Remove(clientId);
    }
}
