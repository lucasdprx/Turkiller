using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class skin_menu : MonoBehaviour
{
    [SerializeField] private skin_selector_script skin_selector;
    [SerializeField] private GameObject buttonType;

    private List<Sprite> sprites;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprites = skin_selector.skins;

        foreach (Sprite sprite in sprites)
        {
            GameObject current_skin = Instantiate(buttonType);
            current_skin.transform.SetParent(transform);
            Image image = current_skin.GetComponent<Image>();
            image.sprite = sprite;
        }
    }
    
    
}
