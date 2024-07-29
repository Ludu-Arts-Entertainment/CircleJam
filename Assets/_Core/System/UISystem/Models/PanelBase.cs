public class PanelBase : UIBase
{
    public override BaseUITypes BaseUIType => BaseUITypes.Panel;

    public override void Show(IBaseUIData data)
    {
        gameObject.SetActive(true);
        OnShown();
    }


    public override void Hide()
    {
        OnHidden();
    }

    protected override void OnShown()
    {
        Shown?.Invoke();
        Shown = null;
    }

    protected override void OnHidden()
    {
        Hidden?.Invoke();
        Hidden = null;
        gameObject.SetActive(false);
    }
}