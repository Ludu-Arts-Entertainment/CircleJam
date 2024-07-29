#if IAPManager_Enabled
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPProductManagementProvider : IProductManagementProvider, IStoreListener
{
    private Action _onPurchaseComplete;
    private Action _onPurchaseFail;
    private IStoreController _controller;
    private IExtensionProvider _extensionsProvider;
    private List<ProductTypeBlock> ProductCollections;
    private bool _iapReady;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private string environment = "test";
#else
    private string environment = "production";
#endif
    public IProductManagementProvider CreateSelf()
    {
        return new IAPProductManagementProvider();
    }
    public void Initialize(Action onReady)
    {
        if (ProductCollections != null) return;
        ProductCollections = Resources.Load<ProductCollection>("ProductCollection").ProductCollections;
        IAPInitialize(onReady);
    }

    private async void IAPInitialize(Action onReady)
    {
        var options = new InitializationOptions().SetEnvironmentName(environment);
        await UnityServices.InitializeAsync(options);
#if UNITY_ANDROID
        var module = StandardPurchasingModule.Instance(AppStore.GooglePlay);
#elif UNITY_IOS
        var module = StandardPurchasingModule.Instance(AppStore.AppleAppStore);
#else
        var module = StandardPurchasingModule.Instance(AppStore.NotSpecified);
#endif
#if UNITY_EDITOR
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
        module.useFakeStoreAlways = true;
#endif
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        foreach (var item in GetProductBlocks(PriceType.IAP))
        {
            builder.AddProduct(item.GetStoreID(), item.productType);
        }
        
        if(builder.products.Count > 0) 
            UnityPurchasing.Initialize(this, builder);
        else
        {
            Debug.Log("No IAP Product Found");
        }

        Debug.Log("Waiting IAP Initialize");
        
        
        var isCanceled = await UniTask.WaitUntil(() => _iapReady, cancellationToken: new CancellationTokenSource(5000).Token).SuppressCancellationThrow();
        if (isCanceled)
        {
            _iapReady = true;
            onReady?.Invoke();
            Debug.LogError("IAP Initialize Timeout");
            return;
        }
        Debug.Log("End IAP Initialize. Result"+_iapReady);
        
        foreach (ProductTypeBlock productCollection in ProductCollections)
        {
            foreach (ListOfProductBlock productBlocks in productCollection.ListOfProductBlocks)
            {
                if (productBlocks.priceType == PriceType.IAP)
                {
                    var product = GetProductList()
                        .Find(item => item.definition.id == productBlocks.GetStoreID());
#if !UNITY_EDITOR
                        if (product != null)
                        {
                            productBlocks.priceBlock.Amount = (float)product.metadata.localizedPrice;
                            productBlocks.priceBlock.AmountString = product.metadata.isoCurrencyCode;
                            productBlocks.name = product.metadata.localizedTitle;
                            productBlocks.description = product.metadata.localizedDescription;
                        }
#endif
                }
            }
        }
        onReady?.Invoke();
    }

    public List<ProductTypeBlock> GetProductCollections()
    {
        return ProductCollections;
    }
    public ListOfProductBlock GetProductBlock(string id, out ProductBlockType storeType)
    {
        storeType = ProductBlockType.None;
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
#if false//UITypes.ProductPurchaseBlocker
        GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.ProductPurchaseBlocker, null);
        onComplete += () => GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.ProductPurchaseBlocker);
        onFailed += () => GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.ProductPurchaseBlocker);
#endif  
        if (CanPurchase(productId).Item1 != PurchaseCheckResult.Success)
        {
            ShowText("Not Available!");
            onComplete?.Invoke();
            return;
        }

        var product = GetProductBlock(productId, out var storeType);
        var tempProductId = productId + (string.IsNullOrEmpty(customId) ? "" : "+" + customId);
        
        if (product.priceType == PriceType.IAP)
        {
            Purchase(product.GetStoreID(), OnComplete, OnFail);
        }
        else
        {
            PayerService.Pay(product, OnComplete, OnFail);
        }

        void OnComplete()
        {
            GiverService.Give(product.listOfProductBlocks, () =>
            {
                var allHistory = GameInstaller.Instance.SystemLocator.DataManager.GetData<List<Dictionary<string, List<StoreTransaction>>>>(GameDataType.StoreTransactionHistory);
                var history = new List<StoreTransaction>() { new StoreTransaction() { Amount = 1, TransactionTime = DateTime.Now } };
                var index = allHistory.FindIndex(x => x.ContainsKey(tempProductId));
                if (index == -1)
                {
                    allHistory.Add(new Dictionary<string, List<StoreTransaction>>() { { tempProductId, history } });
                }
                else
                    allHistory[index][tempProductId].Add(new StoreTransaction() { Amount = 1, TransactionTime = DateTime.Now });

                GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.StoreTransactionHistory, allHistory);
                GameInstaller.Instance.SystemLocator.DataManager.SaveData();
                onComplete?.Invoke();
                ShowText("Purchase Succeed!");
            });
        }
        void OnFail()
        {
            ShowText("Purchase Failed!");
            GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide, "Purchase Failed!"));
            onFailed?.Invoke();
        }

        
    }
    private void Purchase(string id, Action onComplete, Action onFail)
    {
        _onPurchaseComplete = onComplete;
        _onPurchaseFail = onFail;
        if (_controller == null)
        {
            Debug.Log("IAPManager is not initialized yet.");
            onFail?.Invoke();
            return;
        }

        var product = _controller.products.WithID(id);
        if (product != null && product.availableToPurchase)
        {
            _controller.InitiatePurchase(product);
        }
        else
        {
            if (product.appleProductIsRestored)
            {
                _onPurchaseComplete?.Invoke();
                Debug.Log("Restored Purchased");
                
                var state = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string, ulong>>(GameDataType.State);
                if (!state.TryAdd("IAPCounter",1))state["IAPCounter"]++;
                GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.State,state);
                GameInstaller.Instance.SystemLocator.DataManager.SaveData();
            }
            else
            {
                Debug.Log(
                    "BuyProduct FAIL. Not purchasing product, either is not found or is not available for purchase");
                onFail?.Invoke();
            }
        }
    } // IAP Purchase
    private void ShowText(string text)
    {
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(
            new Events.OnFloatingTextAnimationStart(FloatingTextType.MoveUpAndHide, text));
    }
    public (PurchaseCheckResult, TimeSpan) CanPurchase(string productId)
    {
        var product = GetProductBlock(productId, out var storeType);
        // if (product.PriceType==PriceType.IAP)
        // {
        //     var iapProduct = IAPManager.Instance.GetProductList().Find(item => item.definition.id == productId);
        //     if (iapProduct.hasReceipt)
        //     {
        //         Debug.Log("Already Purchased!");
        //         return false;
        //     }
        // }
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
            index = allHistory.Count - 1;
            GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.StoreTransactionHistory, allHistory);
        }

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
    public Sprite GetProductIcon(ProductBlockType type, ProductBlockSubType subType)
    {
        var sprite = Resources.Load<Sprite>($"ProductIcons/{type}/{subType}");
        return sprite ? sprite : null;
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
    #region IAP_StoreCallback
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        _extensionsProvider = extensions;
        Debug.LogWarning("unity purchasing initialized");
        _iapReady = true;
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Error initializing IAP because of {error} without Message." +
                       $"\r\n Show a message to the player depending on the error");
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("Error initializing IAP because of " + error + "\r\n" + message +
                       "\r\n Show a message to the player depending on the error");
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log($"Failed to purchase {i.definition.id} because {p}");
        _onPurchaseFail?.Invoke();
        _onPurchaseFail = null;
    }
    public void OnPurchaseFailed(Product i, PurchaseFailureDescription p)
    {
        Debug.Log($"Failed to purchase {i.definition.id} because {p}");
        _onPurchaseFail?.Invoke();
        _onPurchaseFail = null;
    }
    #endregion
    #region IAP_ProductList
    public List<Product> GetProductList()
    {
        if (_controller == null)
        {
            Debug.LogWarning("IAPManager is not initialized yet. Caller GetProductList will initialize it.");
            Initialize(null);
        }
        //Debug.LogWarning("get product list" + _controller?.products.all.ToList().Count);

        return _controller?.products?.all?.ToList();
    }
    #endregion
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        _onPurchaseComplete?.Invoke();
        _onPurchaseComplete = null;
        Debug.Log($"Purchased product {e.purchasedProduct.definition.id}.");
        return PurchaseProcessingResult.Complete;
    }
}
#endif