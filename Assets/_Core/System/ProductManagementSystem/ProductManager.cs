using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : IManager
{
    private IProductManagementProvider _provider; 
    public IManager CreateSelf()
    {
        return new ProductManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _provider = ProductManagementProviderFactory.Create(gameInstaller.Customizer.productManagementProvider);
        _provider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _provider != null;
    }
    
    public List<ProductTypeBlock> GetProductCollections()
    {
        return _provider.GetProductCollections();
    }
    public ListOfProductBlock GetProductBlock(string id, out ProductBlockType storeType)
    {
        return _provider.GetProductBlock(id, out storeType);
    }
    public List<ListOfProductBlock> GetProductBlocks(ProductBlockType productStoreType)
    {
        return _provider.GetProductBlocks(productStoreType);
    }
    public List<ListOfProductBlock> GetProductBlocks(PriceType priceType)
    {
        return _provider.GetProductBlocks(priceType);
    }

    public void Purchase(string productId, Action onComplete, Action onFailed = null, string customId = null)
    {
        _provider.Purchase(productId, onComplete, onFailed,customId);
    }
    public (PurchaseCheckResult, TimeSpan) CanPurchase(string productId)
    {
        return _provider.CanPurchase(productId);
    }
    public Sprite GetProductIcon(ProductBlockType type, ProductBlockSubType subType)
    {
        return _provider.GetProductIcon(type, subType);
    }
}

public class StoreTransaction
{
    public int Amount { get; set; }
    public DateTime TransactionTime { get; set; }
}
public enum PurchaseCheckResult
{
    Success,
    Failed,
    AlreadyPurchased,
    NotAvailable,
    TimeLimit
}