using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicProductManagementProvider : IProductManagementProvider
{
    private List<ProductTypeBlock> ProductCollections = null;

    public IProductManagementProvider CreateSelf()
    {
        return new BasicProductManagementProvider();
    }

    public void Initialize(Action onReady)
    {
        if (ProductCollections != null) return;
        ProductCollections = Resources.Load<ProductCollection>("ProductCollection")?.ProductCollections;
        onReady?.Invoke();
    }

    public List<ProductTypeBlock> GetProductCollections()
    {
        return ProductCollections;
    }
    public Sprite GetProductIcon(ProductBlockType type, ProductBlockSubType subType)
    {
        var sprite = Resources.Load<Sprite>($"ProductIcons/{type}/{subType}");
        return sprite ? sprite : null;
    }

    public ListOfProductBlock GetProductBlock(string id, out ProductBlockType storeType)
    {
        storeType = ProductBlockType.NoAds;
        foreach (var productCollection in ProductCollections)
        {
            storeType = productCollection.productStoreType;
            foreach (var productBlocks in productCollection.ListOfProductBlocks)
            {
                if (productBlocks.id == id)
                {
                    return productBlocks;
                }
            }
        }

        return null;
    }

    public List<ListOfProductBlock> GetProductBlocks(ProductBlockType productStoreType)
    {
        foreach (var productCollection in ProductCollections)
        {
            if (productCollection.productStoreType == productStoreType)
            {
                return productCollection.ListOfProductBlocks;
            }
        }

        return null;
    }

    public List<ListOfProductBlock> GetProductBlocks(PriceType priceType)
    {
        var list = new List<ListOfProductBlock>();
        foreach (var productCollection in ProductCollections)
        {
            foreach (var productBlocks in productCollection.ListOfProductBlocks)
            {
                if (productBlocks.priceType == priceType)
                {
                    list.Add(productBlocks);
                }
            }
        }

        return list;
    }

    public void Purchase(string productId, Action onComplete, Action onFailed = null, string customId = null)
    {
        if (CanPurchase(productId).Item1 != PurchaseCheckResult.Success)
        {
            ShowFailed("Not Available!");
            onComplete?.Invoke();
            return;
        }

        var tempProductId = productId + (string.IsNullOrEmpty(customId) ? "" : "+" + customId);
        var product = GetProductBlock(productId, out var storeType);
        PayerService.Pay(product,
            () =>
            {
                GiverService.Give(product.listOfProductBlocks, () =>
                {
                    var allHistory = GameInstaller.Instance.SystemLocator.DataManager
                        .GetData<List<Dictionary<string, List<StoreTransaction>>>>(
                            GameDataType.StoreTransactionHistory);
                    var history = new List<StoreTransaction>()
                    {
                        new ()
                        {
                            Amount = 1,
                            TransactionTime = DateTime.Now
                        }
                    };
                    var index = allHistory.FindIndex(x => x.ContainsKey(tempProductId));
                    if (index == -1)
                    {
                        allHistory.Add(new Dictionary<string, List<StoreTransaction>>()
                        {
                            { tempProductId, history }
                        });
                    }
                    else
                        allHistory[index][tempProductId].Add(new StoreTransaction()
                        {
                            Amount = 1,
                            TransactionTime = DateTime.Now
                        });

                    GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.StoreTransactionHistory,
                        allHistory);
                    GameInstaller.Instance.SystemLocator.DataManager.SaveData();
                    GameInstaller.Instance.SystemLocator.HapticManager.Success();
                    onComplete?.Invoke();
                });
            },
            () =>
            {
                Debug.Log("Purchase Failed!");
                ShowFailed("Purchase Failed!");
                GameInstaller.Instance.SystemLocator.HapticManager.Failure();
                onFailed?.Invoke();
            });
    }

    private void ShowFailed(string text)
    {
        // Base.Systems.TutorialSystem.Scripts.Controllers.VisualController.PopupTextController.Show(Vector3.zero, text,
        //     Color.white);
        if (CoroutineController.IsCoroutineRunning("PurchaseFailTextRoutineKey"))
        {
            CoroutineController.StopCoroutine("PurchaseFailTextRoutineKey");
            //Base.Systems.TutorialSystem.Scripts.Controllers.VisualController.PopupTextController.Hide();
        }

        CoroutineController.StartCoroutine("PurchaseFailTextRoutineKey", WaitNHide());
    }

    private IEnumerator WaitNHide()
    {
        yield return new WaitForSeconds(1f);
        //Base.Systems.TutorialSystem.Scripts.Controllers.VisualController.PopupTextController.Hide();
    }

    public (PurchaseCheckResult, TimeSpan) CanPurchase(string productId)
    {
        var product = GetProductBlock(productId, out var storeType);

        if (product.isAvailable == false)
        {
            Debug.Log("Not Available!");
            return (PurchaseCheckResult.NotAvailable, TimeSpan.Zero);
        }

        var allHistory = GameInstaller.Instance.SystemLocator.DataManager
            .GetData<List<Dictionary<string, List<StoreTransaction>>>>(
                GameDataType.StoreTransactionHistory);
        var history = new List<StoreTransaction>();
        var index = allHistory.FindIndex(x => x.ContainsKey(productId));
        if (index == -1)
        {
            allHistory.Add(new Dictionary<string, List<StoreTransaction>>()
            {
                { product.id, history }
            });
            GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.StoreTransactionHistory, allHistory);
            GameInstaller.Instance.SystemLocator.DataManager.SaveData();
        }
        else
            history = allHistory[index][productId];

        if (history.Count >= product.availableCount && product.availableCount != -1 && product.availableTime <= 0)
        {
            Debug.Log("Already Purchased!");
            return (PurchaseCheckResult.AlreadyPurchased, TimeSpan.Zero);
        }

        GetPurchasableTime(history, product, out TimeSpan time);
        if (time <= TimeSpan.Zero) return (PurchaseCheckResult.Success, TimeSpan.Zero);
        return (PurchaseCheckResult.TimeLimit, time);
    }

    public void GetPurchasableTime(List<StoreTransaction> history, ListOfProductBlock product, out TimeSpan time)
    {
        if (product.availableTime <= 0)
        {
            time = TimeSpan.Zero;
            return;
        }

        if (history.Count < product.availableCount || history.Count == 0)
        {
            time = TimeSpan.Zero;
            return;
        }

        var availableCount = history.Count < product.availableCount ? history.Count : product.availableCount;
        var lastTransactionTimes = history.GetRange(history.Count - availableCount, availableCount);
        var storeTransactions = lastTransactionTimes.OrderBy(x => x.TransactionTime);
        var timeSpan = storeTransactions.First().TransactionTime.AddSeconds(product.availableTime) - DateTime.Now;
        time = timeSpan <= TimeSpan.Zero ? TimeSpan.Zero : timeSpan;
    }
}