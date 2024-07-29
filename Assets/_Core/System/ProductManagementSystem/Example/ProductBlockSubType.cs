#if !ProductManager_Modified
public enum ProductBlockSubType
{
    None,
    [SubcategoryOf(ProductBlockType.Currency)]
    Gem,
   [SubcategoryOf(ProductBlockType.Currency)]
    Coin,
   [SubcategoryOf(ProductBlockType.NoAds)]
    NoAds
}
#endif