using TMPro;
using UnityEngine;

public abstract class BaseProductBanner : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    RectTransform _rectTransform;
    
    public void Initialize(ProductBlockType storeType,string title,string description)
    {
        _title.SetText(title);
        _description.SetText(description);
        _rectTransform = GetComponent<RectTransform>();
    }
    public void SetWidth(float width)
    {
        var rect = _rectTransform.rect;
        rect.width = width;
        _rectTransform.sizeDelta = rect.size;
    }
}
