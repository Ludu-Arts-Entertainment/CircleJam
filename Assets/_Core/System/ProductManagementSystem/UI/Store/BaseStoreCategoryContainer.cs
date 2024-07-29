using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStoreCategoryContainer : MonoBehaviour
{
    private ProductBlockType _storeType;
    public Transform contentTransform;
    [HideInInspector]public RectTransform rectTransform;
    private BaseProductBanner _banner;
    private List<BaseStoreProduct> _products = new List<BaseStoreProduct>();
    public virtual void Initialize(ProductBlockType storeType,string title,string description)
    {
        _storeType = storeType;
        rectTransform = GetComponent<RectTransform>();
        _banner = InstantiateBanner(storeType);
        _banner.Initialize(storeType,title,description);
        _banner.SetWidth(rectTransform.rect.width);
    }

    public void CreateProducts(ProductBlockType storeType, List<ListOfProductBlock> products)
    {
        foreach (var product in products)
        {
            var tmp_product = InstantiateProduct(storeType);
            tmp_product.Initialize(product);
            tmp_product.OnPurchase += (model, onComplete, onFailed) =>
            {
                GameInstaller.Instance.SystemLocator.ProductManager.Purchase(model.id, onComplete, onFailed);
            };
            _products.Add(tmp_product);
        }
    }
    private BaseProductBanner InstantiateBanner(ProductBlockType storeType)
    {
        var banner = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<BaseProductBanner>($"{storeType}_Store_Banner", parent: transform);
        banner.transform.SetSiblingIndex(0);
        return banner;
    }

    private BaseStoreProduct InstantiateProduct(ProductBlockType storeType)
    {
        var product = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<BaseStoreProduct>($"{storeType}_Store_Product", parent:contentTransform);
        product.transform.localScale = Vector3.one;
        return product;
    }
}