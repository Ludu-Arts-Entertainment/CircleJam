using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class GiverService
{
    private static readonly Dictionary<ProductBlockType, IGiver> Givers = new Dictionary<ProductBlockType, IGiver>();
    public static void Give(List<ProductBlock> productBlocks, Action onComplete, Action onFail = null)
    {
        if (productBlocks == null || productBlocks.Count == 0)
        {
            onComplete.Invoke();
            return;
        }
        foreach (var product in productBlocks)
        {
            var giver = GetGiver(product.type);
            giver.Give(product, onComplete, onFail);
        }
    }
    private static IGiver GetGiver(ProductBlockType productBlockType)
    {
        if (Givers.TryGetValue(productBlockType, out var giver))
        {
            return giver;
        }
        giver = GameInstaller.Instance.Givers.Find(x => x.productBlockType == productBlockType)?.giver;
        if (giver == null)
        {
            Debug.Log("Giver not found for " + productBlockType);
            return null;
        }
        Givers.Add(productBlockType, giver);
        return giver;
    }
}
[Serializable]
public class ProductBlockTypeGiverTuple
{
    public ProductBlockType productBlockType;
    public BaseGiver giver;
}

