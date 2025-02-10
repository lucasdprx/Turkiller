using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public void OpenUI(UI ui)
    {
        ui.gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OpenScene(string scene)
    {
        //SceneManager.LoadScene(scene);
    }
}
