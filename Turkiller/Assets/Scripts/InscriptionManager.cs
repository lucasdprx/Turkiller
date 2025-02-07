using TMPro;
using UnityEngine;

public class InscriptionManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public GameObject objectToDesactiveOnPlay;
    public NetworkMenu networkMenu;

    public void StartGame()
    {
        string newName = nameText.text;

        ReferenceManager.instance.playerName = newName;

        networkMenu.StartClient(objectToDesactiveOnPlay);
    }

    public void HostGame()
    {
        string newName = nameText.text;

        ReferenceManager.instance.playerName = newName;

        networkMenu.StartHost(objectToDesactiveOnPlay);
    }
}
