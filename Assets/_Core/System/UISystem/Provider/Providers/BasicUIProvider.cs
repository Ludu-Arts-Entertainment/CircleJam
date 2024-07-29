using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public enum BaseUITypes
{
    Panel,
    Popup,
    Overlay,
    Blocker
}

public class BasicUIProvider : IUIProvider
{
    private Dictionary<string, List<UIBase>> _freeUIBases = new();
    public Dictionary<string, List<UIBase>> _busyUIBases = new();
    
    private Queue<(string, IBaseUIData)> _queueUIBases = new();
    private Transform _uiRoot;
    private Dictionary<BaseUITypes, Transform> _uiCanvasDict = new();
    private Dictionary<string, UIBase> UIReferences = new();
    
    private Dictionary<BaseUITypes, int> _uiActiveBlockerCounter = new();

    public Dictionary<BaseUITypes, List<string>> BusyUIBases { get; set; } = new();
    
    private List<BaseUITypes> _uiBases = new List<BaseUITypes>()
    
    {
        BaseUITypes.Panel,
        BaseUITypes.Overlay,
        BaseUITypes.Popup,
        BaseUITypes.Blocker
    };
    
    public IUIProvider CreateSelf()
    {
        return new BasicUIProvider();
    }

    public void Initialize(Action onReady)
    {
        _uiRoot = new GameObject("UISystem").transform;
        Object.DontDestroyOnLoad(_uiRoot);
        var canvasPrefab = Resources.Load<GameObject>("UI/BaseCanvas");
        if(canvasPrefab == null)
        {
            Debug.LogWarning("BaseCanvas prefab not found");
            return;
        }
        for (int i = 1; i <= _uiBases.Count; i++)
        {
            var canvas = Object.Instantiate(canvasPrefab, _uiRoot);
            canvas.name = $"{_uiBases[i - 1]}Canvas";
            canvas.GetComponent<Canvas>().sortingOrder = i * 50;
            canvas.GetComponent<Canvas>().worldCamera = Camera.main;
            canvas.GetComponent<Canvas>().planeDistance = 50;
            _uiCanvasDict.Add(_uiBases[i - 1], canvas.transform);
        }

        foreach (var uiBase in _uiBases)
        {
            var uiPrefab = Resources.LoadAll<UIBase>($"UI/{uiBase.ToString()}");
            foreach (var pair in uiPrefab)
            {
                UIReferences.Add(pair.name, pair);
            }
        }

        onReady.Invoke();
    }

    public void Show(string uiType, IBaseUIData data)
    {

        if (_freeUIBases.TryGetValue(uiType, out var uiBase))
        {
            if (uiBase.Count > 0)
            {
                uiBase[0].transform.SetSiblingIndex(_uiCanvasDict[uiBase[0].BaseUIType].childCount);
                uiBase[0].Show(data);
                _busyUIBases.TryAdd(uiType, new List<UIBase>());
                _busyUIBases[uiType].Add(uiBase[0]);
                
                BusyUIBases.TryAdd(uiBase[0].BaseUIType, new List<string>());
                BusyUIBases[uiBase[0].BaseUIType].Add(uiType);
                _freeUIBases[uiType].RemoveAt(0);
                if (_freeUIBases[uiType].Count! > 0)
                {
                    _freeUIBases.Remove(uiType);
                }
                return;
            }
        }

        if (UIReferences.TryGetValue(uiType, out var uiPrefab))
        {
            var ui = Object.Instantiate(uiPrefab, _uiCanvasDict[uiPrefab.BaseUIType]);
            ui.Show(data);
            _busyUIBases.TryAdd(uiType, new List<UIBase>());
            _busyUIBases[uiType].Add(ui);
            BusyUIBases.TryAdd(ui.BaseUIType, new List<string>());
            BusyUIBases[ui.BaseUIType].Add(uiType);
        }
        else Debug.LogError($"UI with type {uiType} not found");
    }

    public void Hide(string uiType)
    {
        if (!_busyUIBases.ContainsKey(uiType) || _busyUIBases[uiType].Count == 0)
        {
            Debug.LogWarning($"There is not an active content!");
            return;
        }

        var uiBase = _busyUIBases[uiType][_busyUIBases[uiType].Count - 1];

        if (_queueUIBases.TryDequeue(out var result))
        {
            uiBase.Hidden += () => Show(result.Item1, result.Item2);
            uiBase.Hide();
        }
        else
            uiBase.Hide();

        _freeUIBases.TryAdd(uiType, new List<UIBase>());
        _freeUIBases[uiType].Add(uiBase);
        _busyUIBases[uiType].Remove(uiBase);
        if (_busyUIBases[uiType].Count == 0)
        {
            _busyUIBases.Remove(uiType);
        }

        if (BusyUIBases.ContainsKey(uiBase.BaseUIType))
        {
            BusyUIBases[uiBase.BaseUIType].Remove(uiType);
        }
    }

    public void AddQueue(string uiType, IBaseUIData data)
    {
        if (_busyUIBases.Count > 0)
        {
            _queueUIBases.Enqueue((uiType, data));
            return;
        }

        Show(uiType, data);
    }

    public void Switch(string uiType, IBaseUIData data)
    {
        var uiBaseType = UIReferences[uiType].BaseUIType;
        HideAll(uiBaseType);
        Show(uiType, data);
    }

    public void HideAll()
    {
        _queueUIBases.Clear();
        foreach (var key in _busyUIBases.Keys)
        {
            foreach (var pair in _busyUIBases[key])
            {
                pair.Hide();
                _freeUIBases.TryAdd(key, new List<UIBase>());
                _freeUIBases[key].Add(pair);
            }
        }

        _busyUIBases.Clear();
    }

    public void HideAll(string uiType)
    {
        Debug.Log("[UI System] Hide All by type " + uiType);

        if (!_busyUIBases.ContainsKey(uiType))
        {
            Debug.LogWarning("UI with this type not found");
            return;
        }

        var uiBases = new List<UIBase>();
        uiBases.AddRange(_busyUIBases[uiType]);

        foreach (var pair in uiBases)
        {
            Hide(uiType);
        }
    }

    public void HideAll(BaseUITypes uiType)
    {
        var dic = new Dictionary<string, List<UIBase>>();
        foreach (var key in _busyUIBases.Keys)
        {
            if (_busyUIBases[key].Count == 0)
            {
                continue;
            }

            if (uiType != _busyUIBases[key][0].BaseUIType)
            {
                continue;
            }

            foreach (var pair in _busyUIBases[key])
            {
                dic.TryAdd(key, new List<UIBase>());
                dic[key].Add(pair);
            }
        }

        foreach (var t in dic.Keys)
        {
            for (int j = 0; j < dic[t].Count; j++)
            {
                Hide(t);
            }
        }
    }
    public void ActivateBlocker(BaseUITypes baseUITypes)
    {
        if (!_uiActiveBlockerCounter.TryAdd(baseUITypes,1))
        {
            _uiActiveBlockerCounter[baseUITypes]++;
        }
        _uiCanvasDict[baseUITypes].GetChild(0).gameObject.SetActive(true);
    }

    public void DeactivateBlocker(BaseUITypes baseUITypes)
    {
        if (_uiActiveBlockerCounter.TryAdd(baseUITypes, 0))
        {
            _uiCanvasDict[baseUITypes].GetChild(0).gameObject.SetActive(false);
            return;
        }
        _uiActiveBlockerCounter[baseUITypes]--;
        _uiActiveBlockerCounter[baseUITypes] = Mathf.Max(0, _uiActiveBlockerCounter[baseUITypes]);
        if (_uiActiveBlockerCounter[baseUITypes]>0)return;
        _uiCanvasDict[baseUITypes].GetChild(0).gameObject.SetActive(false);
    }

}