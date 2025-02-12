using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine.UI;

public class skin_selector_script : UI
{
    public List<Sprite> skins;
    [SerializeField] private GameObject leftSkin;
    [SerializeField] private GameObject rightSkin;
    [SerializeField] private GameObject middleSkin;

    private Image leftRenderer;
    private Image rightRenderer;
    private Image middleRenderer;

    private int currentSkin = 0;

    void Start()
    {
        leftRenderer = leftSkin.GetComponent<Image>();
        rightRenderer = rightSkin.GetComponent<Image>();
        middleRenderer = middleSkin.GetComponent<Image>();
        
        ChangeSkinTo(0);
    }

    public void ChangeSkin(bool left)
    {
        List<int> index = SkinUpdate(left);

        leftRenderer.sprite = skins[index[0]];
        middleRenderer.sprite = skins[index[1]];
        rightRenderer.sprite = skins[index[2]];
    }

    private List<int> SkinUpdate(bool left)
    {
        int i = 0;
        
        List<int> results = new List<int>();
        if (left)
        {
            currentSkin--;
            i = currentSkin;

            if (i - 1 < 0)
            {
                results.Add(skins.Count + i - 1);
            }
            else
            {
                results.Add(i - 1);
            }

            if (currentSkin < 0)
            {
                currentSkin = skins.Count - 1;
            }
            results.Add(currentSkin);

            results.Add(i + 1);
        }
        else
        {
            currentSkin++;
            i = currentSkin;

            results.Add(i - 1);

            if (currentSkin >= skins.Count)
            {
                currentSkin = 0;
            }
            results.Add(currentSkin);

            if (i + 1 >= skins.Count)
            {
                results.Add(i + 1 - skins.Count);
            }
            else
            {
                results.Add(i + 1);
            }
        }

        return results;
    }

    private void ChangeSkinTo(int index)
    {
        currentSkin = index;
        ChangeSkin(true);
        ChangeSkin(false);
    }
}