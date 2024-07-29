using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicChestProductElement : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    public void Set(ProductBlock productBlock)
    {
        var color = icon.color;
        color.a = 1;
        icon.color = color;
        icon.sprite = GameInstaller.Instance.SystemLocator.ProductManager.GetProductIcon(productBlock.type, productBlock.subType);
        countText.text = productBlock.amount.ToString();
    }

    [NaughtyAttributes.Button]
    public void Reset()
    {
        var color = icon.color;
        color.a = 0;
        icon.color = color;
        icon.sprite = null;
        countText.text = "";
    }
}
