using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteTab : MonoBehaviour
{
   [SerializeField] private GameObject gainedObject;
   [SerializeField] private Image iteSprite;
   [SerializeField] private TextMeshProUGUI amountText;

    public RouletteSaveData RouletteData => rouletteData;
    private RouletteSaveData rouletteData;

    private void OnEnable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnRouletteItemGain>(OnRouletteItemGain);
    }
    private void OnDisable() 
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnRouletteItemGain>(OnRouletteItemGain);
    }

    private void OnRouletteItemGain(Events.OnRouletteItemGain gain)
    {
        if(gain.Id == rouletteData.RouletteDataId)
        {
            rouletteData = GameInstaller.Instance.SystemLocator.RouletteManager.GetRouletteItem(rouletteData.RouletteDataId);
            SetGained();
        }
    }

    public void LoadTab(int rouletteDataId)
   {
        rouletteData = GameInstaller.Instance.SystemLocator.RouletteManager.GetRouletteItem(rouletteDataId);
        if(rouletteData == null) return;

        iteSprite.sprite = GameInstaller.Instance.SystemLocator.ProductManager.GetProductIcon(rouletteData.ProductBlock.type, rouletteData.ProductBlock.subType);
        amountText.text = $"{rouletteData.ProductBlock.amount}";

        SetGained();
   }

   public void SetGained()
   {
        gainedObject.SetActive(rouletteData.IsGained);
   }
}
