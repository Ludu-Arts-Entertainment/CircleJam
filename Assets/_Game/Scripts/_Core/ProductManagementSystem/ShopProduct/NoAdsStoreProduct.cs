using System.Globalization;
using TMPro;
using UnityEngine;

public class NoAdsStoreProduct : BaseStoreProduct
{
    [SerializeField] private TMP_Text sideRewardAmountText;

    public override void Initialize(ListOfProductBlock model)
    {
        base.Initialize(model);
        sideRewardAmountText.text = model.listOfProductBlocks[1].amount.ToString(CultureInfo.InvariantCulture);
    }
}
