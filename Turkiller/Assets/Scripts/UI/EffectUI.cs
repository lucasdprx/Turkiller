using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    [SerializeField] private Bonus _bonus;

    [Serializable]public struct SeedSprites
    { 
        public Sprite sprite; 
        public Bonus bonus;
    }

    private PlayerEffects _playerEffects;
    public List<SeedSprites> seedSprites = new();
    public Image imageEffect;
    public Image bgImage;

   public void Init(PlayerEffects playerEffects, Bonus bonus)
    {
        _bonus = bonus;
        _playerEffects = playerEffects;
        for(int i = 0; i < seedSprites.Count; i++)
        {
            if(bonus == seedSprites[i].bonus)
            {
                imageEffect.sprite = seedSprites[i].sprite;
                return;
            }
        }
    }

    private void Update()
    {
        var timer = _playerEffects.HighestTimer(_bonus);
        bgImage.fillAmount = timer.time / timer.max;


        if(timer.time <= 0) Destroy(gameObject);
    }
}
