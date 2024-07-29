public class ShopPanel : PanelBase
{
    private StoreController _storeController;
    public override void Show(IBaseUIData data)
    {
        _storeController = GetComponent<StoreController>();
        _storeController.Initialize();
        base.Show(data);
    }
}
public partial class UITypes
{
    public const string ShopPanel = nameof(ShopPanel);
}