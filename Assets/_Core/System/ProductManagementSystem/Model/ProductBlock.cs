using System.Linq;
using NaughtyAttributes;


[System.Serializable]
public class ProductBlock
{
    public ProductBlockType type;
    [Dropdown("GetSubtypeValues")] public ProductBlockSubType subType;
    public float amount;

    public ProductBlock()
    { }
    public ProductBlock( ProductBlockType type, ProductBlockSubType subType, float amount)
    {
        this.type = type;
        this.subType = subType;
        this.amount = amount;
    }
    private DropdownList<ProductBlockSubType> GetSubtypeValues()
    {
        var list = new DropdownList<ProductBlockSubType>();
        foreach (var pair in type.GetSubcategories<ProductBlockSubType>())
        {
            list.Add(pair.ToString(), (ProductBlockSubType)pair);
        }
        return list;
    }
}