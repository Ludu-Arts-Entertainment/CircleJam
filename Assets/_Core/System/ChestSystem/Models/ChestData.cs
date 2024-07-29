#if !ChestManager_Modified

using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public record ChestData
{
    [Dropdown("GetSubtypeValues")]
    public string ChestType;
    [Range(1,9)]
    public int WillGiveCount = 1;
    public ProductChanceTuple[] ProductChanceTuples;
    public string IconKey => ChestType.ToString()+"_Chest_Icon";
    public ProductBlock GetRandomProductBlock(ProductBlock defProductBlock = null)
    {
        if (ProductChanceTuples.Length == 0)
        {
            Debug.Log("No products in chest");
            return defProductBlock;
        }
        
        List<ProductChanceTuple> productChanceTuples = new List<ProductChanceTuple>();
        float sumChance = 0;
        foreach (var pair in ProductChanceTuples)
        {
            productChanceTuples.Add(new ProductChanceTuple(pair.Product,sumChance));
            sumChance += pair.Chance;
        }
        var randomChance = Random.Range(0, sumChance);
        for (int i = 0; i < productChanceTuples.Count; i++)
        {
            if (i==productChanceTuples.Count-1)
            {
                return productChanceTuples[i].Product;
            }
            if (productChanceTuples[i].Chance<=randomChance&&productChanceTuples[i+1].Chance>randomChance)
            {
                return productChanceTuples[i].Product;
            }
        }
        return defProductBlock;
    }
    private DropdownList<string> GetSubtypeValues()
    {
        var list = new DropdownList<string>();
        foreach (var str in (typeof(ChestType)).GetAllPublicConstantValues<string>())
        {
            list.Add(str,str);
        }
        return list;
    }
}
#endif
[System.Serializable]
public record ProductChanceTuple
{
    public ProductBlock Product;
    public float Chance;
    public ProductChanceTuple(ProductBlock product, float chance)
    {
        Product = product;
        Chance = chance;
    }
}
