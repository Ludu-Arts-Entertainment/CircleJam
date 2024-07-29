using System;
using System.Collections.Generic;
using UnityEngine;

public interface IProductManagementProvider
{
    IProductManagementProvider CreateSelf();
    void Initialize(Action onReady);
    List<ProductTypeBlock> GetProductCollections();
    ListOfProductBlock GetProductBlock(string id, out ProductBlockType storeType);
    List<ListOfProductBlock> GetProductBlocks(ProductBlockType productStoreType);
    List<ListOfProductBlock> GetProductBlocks(PriceType priceType);
    void Purchase(string productId, Action onComplete, Action onFailed = null, string customId = null);
    (PurchaseCheckResult, TimeSpan) CanPurchase(string productId);
    Sprite GetProductIcon(ProductBlockType type, ProductBlockSubType subType);
}