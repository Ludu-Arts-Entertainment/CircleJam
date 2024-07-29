using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicProgressSummaryCardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _coins;
    
    [SerializeField] private Button _selectButton;
    [SerializeField] private TextMeshProUGUI _selectButtonText;

    public Button SelectButton => _selectButton;
    public BasicProgressSummaryCardModel Data => _data;
    
    private BasicProgressSummaryCardModel _data;

    public void SetData(BasicProgressSummaryCardModel data)
    {
        _data = data;
        _title.text = _data.title;
        _level.text = _data.level.ToString();
        _coins.text = _data.coins.ToString();
        _selectButtonText.text ??= "Select";
    }
}
public class BasicProgressSummaryCardModel
{
    public DataSourceType dataSourceType;
    public string title;
    public int level;
    public int coins;
}
public enum DataSourceType
{
    Local = 0,
    Remote = 1,
}