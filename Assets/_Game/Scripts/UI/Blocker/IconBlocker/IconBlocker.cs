using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBlocker : BlockerBase
{
    private const string DEFAULT_ICON_BLOCKER_TYPE = "ScatterUI";

    [SerializeField]
    private List<IconBlockerTypeByPoolId> iconBlockerTypeByPoolIds = new ();
    private readonly Dictionary<IconAnimationType, string> _blockerTypeToPoolId = new ();
    private readonly Dictionary<Type, Transform> iconTransforms = new();

    private readonly Dictionary<ProductBlockSubType, Type> productSubTypeElementTypeMapper = new()
    {
        // {ProductBlockSubType.Coin, typeof(CoinElement)},
        // {ProductBlockSubType.Energy, typeof(EnergyElement)},
        // {ProductBlockSubType.Star, typeof(StarElement)},
        // {ProductBlockSubType.None, typeof(PlayButtonElement)},
    };

    private void Awake()
    {
        foreach (var item in iconBlockerTypeByPoolIds)
        {
            _blockerTypeToPoolId.TryAdd(item.iconAnimationType, item.poolId);
        }
    }

    public override void Show(IBaseUIData data)
    {
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnUIElementCreated>(OnUIElementCreated);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnUIElementDisposed>(OnUIElementDisposed);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnIconAnimationAuto>(OnIconAnimationAuto);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnIconAnimationCustom>(OnIconAnimationCustom);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnIconAnimationAutoProduct>(OnIconAnimationAutoProduct);
        
        base.Show(data);
    }

    public override void Hide()
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnUIElementCreated>(OnUIElementCreated);
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnUIElementDisposed>(OnUIElementDisposed);
        
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnIconAnimationAuto>(OnIconAnimationAuto);
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnIconAnimationCustom>(OnIconAnimationCustom);
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.OnIconAnimationAutoProduct>(OnIconAnimationAutoProduct);
        
        base.Hide();
    }
    
    private void OnUIElementCreated(Events.OnUIElementCreated obj)
    {
        Debug.Log(obj.Type + "//"+obj.Transform);
        if (!iconTransforms.TryAdd(obj.Type, obj.Transform))
        {
            iconTransforms[obj.Type] = obj.Transform;
        }
    }
    private void OnUIElementDisposed(Events.OnUIElementDisposed obj)
    {
        if (iconTransforms.ContainsKey(obj.Type))
        {
            iconTransforms[obj.Type] = null;
        }
    }
    
    private void OnIconAnimationCustom(Events.OnIconAnimationCustom _event)
    {
        StartCoroutine(ScatterAnimation(_event));
    }
    private void OnIconAnimationAuto(Events.OnIconAnimationAuto _event)
    {
        StartCoroutine(ScatterAnimation(_event));
    }
    private void OnIconAnimationAutoProduct(Events.OnIconAnimationAutoProduct _event)
    {
        StartCoroutine(ScatterAnimation(_event));
    }
    private IEnumerator ScatterAnimation(Events.OnIconAnimationCustom _event)
    {
        _event.startAction?.Invoke();
        ScatterUI scatterUI = null;
        for(int i = 0; i < _event.count; i++)
        {
            var iconType = _blockerTypeToPoolId.TryGetValue(_event.iconAnimationType, out var value) ? value : DEFAULT_ICON_BLOCKER_TYPE;
            scatterUI = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<ScatterUI>(iconType, parent:_event.isWorld ? null : transform);
            scatterUI.transform.position = _event.startPoint;
            scatterUI.SetImage(_event.iconSprite);
            scatterUI.transform.localScale = Vector3.one * _event.startScale;
            scatterUI.StartAnimation(_event.randomCircleRange, _event.moveScale, _event.endPoint, _event.everyIconEndAction);
        }
        yield return new WaitForSeconds(scatterUI.GetTotalAnimationTime());
    
        _event.endAction?.Invoke();
    } // OnIconAnimationCustom
    private IEnumerator ScatterAnimation(Events.OnIconAnimationAuto _event)
    {
        _event.startAction?.Invoke();
        ScatterUI scatterUI = null;
        var targetPos = GetIconPosition(_event.targetType);
        for(int i = 0; i < _event.count; i++)
        {
            var iconType = _blockerTypeToPoolId.TryGetValue(_event.iconAnimationType, out var value) ? value : DEFAULT_ICON_BLOCKER_TYPE;
            scatterUI = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<ScatterUI>(iconType,  parent : transform);
            scatterUI.transform.position = _event.startPoint;
            scatterUI.SetImage(_event.iconSprite);
            scatterUI.transform.localScale = Vector3.one * _event.startScale;
            scatterUI.StartAnimation(_event.randomCircleRange, _event.moveScale, targetPos, _event.everyIconEndAction);
        }
        yield return new WaitForSeconds(scatterUI.GetTotalAnimationTime());
        _event.endAction?.Invoke();
    } // OnIconAnimationAuto
    private IEnumerator ScatterAnimation(Events.OnIconAnimationAutoProduct _event)
    {
        _event.startAction?.Invoke();
        if (_event.productBlock.type==ProductBlockType.NoAds)
        {
            _event.endAction?.Invoke();
            yield break;
        }
        ScatterUI scatterUI = null;
        var targetPos =
            GetIconPosition(productSubTypeElementTypeMapper.TryGetValue(_event.productBlock.subType, out Type type)
                ? type
                : productSubTypeElementTypeMapper[ProductBlockSubType.None]);
        for(int i = 0; i < (_event.productBlock.amount > 10?10:_event.productBlock.amount); i++)
        {
            var iconType = _blockerTypeToPoolId.TryGetValue(_event.iconAnimationType, out var value) ? value : DEFAULT_ICON_BLOCKER_TYPE;
            scatterUI = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<ScatterUI>(iconType,  parent : transform);
            scatterUI.transform.position = _event.startPoint;
            scatterUI.SetImage(
                GameInstaller.Instance.SystemLocator.ProductManager.GetProductIcon(_event.productBlock.type, _event.productBlock.subType));
            scatterUI.transform.localScale = Vector3.one * _event.startScale;
            scatterUI.StartAnimation(_event.randomCircleRange, _event.moveScale, targetPos , _event.everyIconEndAction);
        }
        yield return new WaitForSeconds(scatterUI.GetTotalAnimationTime());
        _event.endAction?.Invoke();
    } // OnIconAnimationAutoProduct
    private Vector3 GetIconPosition(Type targetType)
    {
        iconTransforms.TryGetValue(targetType, value: out var iconTransform);
        return  iconTransform != null ? iconTransform.position : Vector3.zero;
    }
}

[Serializable]
public struct IconBlockerTypeByPoolId
{
    public IconAnimationType iconAnimationType;
    public string poolId;
}

public enum IconAnimationType
{
    ScatterUI = 0,
    ProductScatter = 2,
}

public partial class UITypes
{
    public const string IconBlocker = "IconBlocker";
}

public partial class Events
{
    public struct OnUIElementCreated : IEvent
    {
        public Type Type;
        public Transform Transform;

        public OnUIElementCreated(Type type, Transform transform)
        {
            Type = type;
            Transform = transform;
        }
    }
    public struct OnUIElementDisposed : IEvent
    {
        public Type Type;

        public OnUIElementDisposed(Type type)
        {
            Type = type;
        }
    }
    public struct OnIconAnimationAuto : IEvent
    {
        public IconAnimationType iconAnimationType;
        public Sprite iconSprite;
        public int count;
        public float randomCircleRange;
        public float startScale;
        public float moveScale;
        public Vector3 startPoint;
        public Action startAction;
        public Action everyIconEndAction;
        public Action endAction;
        public Type targetType;

        public OnIconAnimationAuto(IconAnimationType iconAnimationType, Type targetType, Sprite iconSprite, int count,
            float randomCircleRange, Vector3 startPoint, float startScale, float moveScale, Action startAction = null,
            Action endAction = null, Action everyIconEndAction = null)
        {
            this.iconAnimationType = iconAnimationType;
            this.targetType = targetType;
            this.iconSprite = iconSprite;
            this.count = count;
            this.randomCircleRange = randomCircleRange;
            this.startScale = startScale;
            this.moveScale = moveScale;
            this.startPoint = startPoint;
            this.startAction = startAction;
            this.everyIconEndAction = everyIconEndAction;
            this.endAction = endAction;
        }
    }
    public struct OnIconAnimationCustom : IEvent
    {
        public IconAnimationType iconAnimationType;
        public Sprite iconSprite;
        public int count;
        public float randomCircleRange;
        public float startScale;
        public float moveScale;
        public Vector3 startPoint;
        public Action startAction;
        public Action everyIconEndAction;
        public Action endAction;
        public Vector3 endPoint;
        public bool isWorld;

        public OnIconAnimationCustom(IconAnimationType iconAnimationType, Vector3 endPoint, Sprite iconSprite,
            int count, float randomCircleRange, Vector3 startPoint, float startScale, float moveScale,
            Action startAction = null, Action endAction = null, Action everyIconEndAction = null, bool isWorld = false)
        {
            this.iconAnimationType = iconAnimationType;
            this.endPoint = endPoint;
            this.iconSprite = iconSprite;
            this.count = count;
            this.randomCircleRange = randomCircleRange;
            this.startScale = startScale;
            this.moveScale = moveScale;
            this.startPoint = startPoint;
            this.startAction = startAction;
            this.everyIconEndAction = everyIconEndAction;
            this.endAction = endAction;
            this.isWorld = isWorld;
        }
    }
    public struct OnIconAnimationAutoProduct : IEvent
    {
        public IconAnimationType iconAnimationType;
        public ProductBlock productBlock;
        public float randomCircleRange;
        public float startScale;
        public float moveScale;
        public Vector3 startPoint;
        public Action startAction;
        public Action everyIconEndAction;
        public Action endAction;

        public OnIconAnimationAutoProduct(IconAnimationType iconAnimationType, ProductBlock productBlock, float randomCircleRange,
            Vector3 startPoint, float startScale, float moveScale, Action startAction = null, Action endAction = null,
            Action everyIconEndAction = null)
        {
            this.iconAnimationType = iconAnimationType;
            this.productBlock = productBlock;
            this.randomCircleRange = randomCircleRange;
            this.startScale = startScale;
            this.moveScale = moveScale;
            this.startPoint = startPoint;
            this.startAction = startAction;
            this.everyIconEndAction = everyIconEndAction;
            this.endAction = endAction;
        }
    }
}