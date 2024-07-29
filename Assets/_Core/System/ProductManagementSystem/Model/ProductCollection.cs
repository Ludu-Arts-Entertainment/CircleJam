using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ProductCollection", menuName = "ProductCollection", order = 0)]
public class ProductCollection : ScriptableObject
{
    public List<ProductTypeBlock> ProductCollections;
}

[System.Serializable]
public class ProductTypeBlock
{
    public ProductBlockType productStoreType;
    public string Name;
    public string Description;
    public List<ListOfProductBlock> ListOfProductBlocks;
}