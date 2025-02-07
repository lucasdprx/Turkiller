using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderBoardElement : MonoBehaviour
{
    public TextMeshProUGUI placementText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void UpdateTexts(int placement, FixedString64Bytes name, int score)
    {
        placementText.text = placement.ToString();
        nameText.text = name.ToString();
        scoreText.text = score.ToString();
    }
}
