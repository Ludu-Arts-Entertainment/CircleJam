public partial class SystemLocator
{
    private ProductManager _productManager;
    
    public ProductManager ProductManager => _productManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.ProductManager] as ProductManager;
}