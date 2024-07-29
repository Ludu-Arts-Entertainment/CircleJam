using System;
using System.Linq;

public class UIManager : IManager
{
    private IUIProvider _uiProvider;
    public string GetActivePanelType(BaseUITypes baseUITypes)
    {
        if (_uiProvider.BusyUIBases.ContainsKey(baseUITypes) && _uiProvider.BusyUIBases[baseUITypes].Count > 0)
        {
            return _uiProvider.BusyUIBases[baseUITypes].Last();
        }
        return null;
    }

    public IManager CreateSelf()
    {
        return new UIManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _uiProvider = UIProviderFactory.Create(gameInstaller.Customizer.UIProvider);
        _uiProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _uiProvider != null;
    }

    public void Show(string uiType, IBaseUIData data)
    {
        _uiProvider.Show(uiType, data);
    }

    public void AddQueue(string uiType, IBaseUIData data)
    {
        _uiProvider.AddQueue(uiType, data);
    }

    public void Switch(string uiType, IBaseUIData data)
    {
        _uiProvider.Switch(uiType, data);
    }

    public void Hide(string uiType)
    {
        _uiProvider.Hide(uiType);
    }

    public void HideAll()
    {
        _uiProvider.HideAll();
    }

    public void HideAll(string uiType)
    {
        _uiProvider.HideAll(uiType);
    }

    public void HideAll(BaseUITypes baseUITypes)
    {
        _uiProvider.HideAll(baseUITypes);
    }
    public void ActivateBlocker(BaseUITypes baseUITypes)
    {
        _uiProvider.ActivateBlocker(baseUITypes);
    }
    public void DeactivateBlocker(BaseUITypes baseUITypes)
    {
        _uiProvider.DeactivateBlocker(baseUITypes);
    }
}