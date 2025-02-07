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


        networkMenu.StartClient(objectToDesactiveOnPlay, newName);
    }

    public void HostGame()
    {
        string newName = nameText.text;


        networkMenu.StartHost(objectToDesactiveOnPlay, newName);
    }
}
