#if !RouletteManager_Modified

using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BasicRouletteScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPortTransform;
    [SerializeField] private RectTransform contentPanelTransform;
    [SerializeField] private HorizontalLayoutGroup hlg;

    private List<RouletteTab> itemlist = new();
    private List<RouletteTab> dublicatedItems = new();
    private Vector2 oldVelocity;
    private bool isUpdated;
    private bool isLoaded;
    private float itemWidth;
    public void Load(List<RouletteTab> items)
    {
        Clear();
        itemlist = items;
        isUpdated = false;
        oldVelocity = Vector2.zero;

        itemWidth = itemlist[0].GetComponent<RectTransform>().rect.width;

        //int itemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.width / (itemlist[0].rect.width + hlg.spacing));

        for (int i = 0; i < itemlist.Count; i++)
        {
            RouletteTab rt = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<RouletteTab>("RouletteTab", parent: contentPanelTransform);
            rt.transform.localScale = Vector3.one;
            rt.LoadTab(itemlist[i % itemlist.Count].RouletteData.RouletteDataId);
            dublicatedItems.Add(rt);
            rt.transform.SetAsLastSibling();
        }

        for(int i = 0; i < itemlist.Count; i++)
        {
            int num = itemlist.Count - i - 1;
            while(num < 0)
            {
                num += itemlist.Count;
            }
            RouletteTab rt = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<RouletteTab>("RouletteTab", parent: contentPanelTransform);
            rt.transform.localScale = Vector3.one;
            rt.LoadTab(itemlist[num].RouletteData.RouletteDataId);
            dublicatedItems.Add(rt);
            rt.transform.SetAsFirstSibling();
        }

        /*contentPanelTransform.localPosition = new Vector3((0- (itemlist[0].rect.width + hlg.spacing) * itemsToAdd), 
            contentPanelTransform.localPosition.y,
            contentPanelTransform.localPosition.z);*/

        isLoaded = true;
    }

    private void Clear()
    {
        foreach (var item in dublicatedItems)
        {
            GameInstaller.Instance.SystemLocator.PoolManager.Destroy("RouletteTab", item);
        }
        dublicatedItems.Clear();
        itemlist.Clear();
        isLoaded = false;
    }

    private void Update()
    {
        if(!isLoaded) return;
        if(isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = oldVelocity;
        }

        if(contentPanelTransform.localPosition.x > 0)
        {
            Canvas.ForceUpdateCanvases();
            oldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition -= new Vector3(itemlist.Count *(itemWidth + hlg.spacing), 0, 0);
            isUpdated = true;
        }

        else if(contentPanelTransform.localPosition.x < 0 - (itemlist.Count *(itemWidth + hlg.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            oldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition += new Vector3(itemlist.Count *(itemWidth + hlg.spacing), 0, 0);
            isUpdated = true;
        }
    }   

    Tween tween;
    private Action onAnimationComplete;
    public void PlayAnimation(RouletteTab rouletteTab, Action onComplete)
    {
        onAnimationComplete = onComplete;

        tween?.Kill();
        contentPanelTransform.DOKill();
        if(itemlist.Contains(rouletteTab))
        {
            tween = contentPanelTransform.DOLocalMoveX(contentPanelTransform.localPosition.x + 1000, 0.5f).SetEase(Ease.Linear).SetLoops(-1);
            DOVirtual.DelayedCall(3f, () =>
            {
                StopAnimation(itemlist.IndexOf(rouletteTab));
            });
        }
        
    }

    public void StopAnimation(int randomIndex)
    {
        tween?.Kill();
        contentPanelTransform.DOKill();

        var halfIndex = itemlist.Count / 2;
        int targetXPos;

        if(randomIndex > halfIndex)
        {
            targetXPos = (int)-(((randomIndex - halfIndex) * itemWidth) + ((randomIndex - halfIndex) * hlg.spacing));
        }

        else if(randomIndex < halfIndex)
        {
            targetXPos = (int)(((halfIndex - randomIndex) * itemWidth) + ((halfIndex - randomIndex) * hlg.spacing));
        }

        else
        {
            targetXPos = 0;
        }

        contentPanelTransform.DOLocalMoveX(targetXPos, 1f).onComplete += () =>
        {
            OnGainRouletteItem(itemlist[randomIndex]);
            onAnimationComplete?.Invoke();
        };
    } 

    private void OnGainRouletteItem(RouletteTab rouletteTab)
    {
        var rouletteSaveData = rouletteTab.RouletteData;
        var amount = rouletteSaveData.ProductBlock.amount;
        
        var rewardCount = Math.Min((int)amount, 10);
        // GameInstaller.Instance.SystemLocator.EventManager.Trigger(
        //     new Events.OnIconAnimationAutoProduct
        //     (
        //         iconAnimationType : IconAnimationType.ScatterUI,
        //         productBlock: new ProductBlock(){amount = amount, subType = rouletteSaveData.ProductBlock.subType ,type = rouletteSaveData.ProductBlock.type},
        //         startAction : null,
        //         everyIconEndAction : null,
        //         randomCircleRange : 50f, 
        //         moveScale : 0.5f, 
        //         startScale : 0f,
        //         startPoint : rouletteTab.transform.position,
        //         endAction : null
        //     ));
    }
}
#endif
