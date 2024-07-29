using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoreController : MonoBehaviour
{
    [SerializeField] private RectTransform contentTransform;

    private List<(ProductBlockType, BaseStoreCategoryContainer)> _contentTransforms =
        new ();

    private bool _isInitialized = false;

    public void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        var productCollections = GameInstaller.Instance.SystemLocator.ProductManager.GetProductCollections();
        foreach (var productCollection in productCollections)
        {
            BaseStoreCategoryContainer _content = null;
            var storeType = productCollection.productStoreType;
            if (_contentTransforms.Any(item => item.Item1 == storeType) == false)
            {
                var content = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<BaseStoreCategoryContainer>($"{productCollection.productStoreType}_Store_Content",  parent:contentTransform);
                content.gameObject.transform.localScale = Vector3.one;
                content.Initialize(storeType,productCollection.Name,productCollection.Description);
                _contentTransforms.Add((storeType, content));
                _content = content;
            }
            else
            {
                _content = _contentTransforms.Find(item => item.Item1 == storeType).Item2;
            }
            _content.CreateProducts(productCollection.productStoreType, productCollection.ListOfProductBlocks);
        }
    }
}