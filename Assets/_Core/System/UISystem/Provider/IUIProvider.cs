using System;
using System.Collections.Generic;

public interface IUIProvider
{
    IUIProvider CreateSelf();
    void Initialize(Action onReady);
    void Show(string uiType, IBaseUIData data);
    void Hide(string uiType);
    void AddQueue(string uiType, IBaseUIData data);
    void Switch(string uiType, IBaseUIData data);
    void HideAll();
    void HideAll(string uiType);
    void HideAll(BaseUITypes baseUITypes);
    void ActivateBlocker(BaseUITypes baseUITypes);
    void DeactivateBlocker(BaseUITypes baseUITypes);
    
    public Dictionary<BaseUITypes, List<string>> BusyUIBases { get;  set; }
}