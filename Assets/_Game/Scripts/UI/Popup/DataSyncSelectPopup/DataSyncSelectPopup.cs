using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataSyncSelectPopup : PopupBase
{
    private const string Header = "Another save found!";
    private const string Message = "Do you want to\nprogress with your\nremote data?";
    
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _message;
    
    [SerializeField] private BasicProgressSummaryCardView remoteBasicProgressSummaryCardView;
    [SerializeField] private BasicProgressSummaryCardView localBasicProgressSummaryCardView;
    
    private Button _selectRemoteDataButton;
    private Button _selectLocalDataButton;
    
    private DataSyncSelectPopupData _dataSyncSelectPopupData;
    
    public override void Show(IBaseUIData data)
    {
        if (data is not DataSyncSelectPopupData dataSyncSelectPopupData)
            return;
        
        // cache DataSyncSelectPopupData on a variable
        _dataSyncSelectPopupData = dataSyncSelectPopupData;
        
        _header.text = String.IsNullOrEmpty(_dataSyncSelectPopupData.header) ? _dataSyncSelectPopupData.header : Header;
        _message.text = String.IsNullOrEmpty(_dataSyncSelectPopupData.message) ? _dataSyncSelectPopupData.message : Message;
        
        localBasicProgressSummaryCardView.SetData(_dataSyncSelectPopupData.LocalBasicProgressSummaryCardData);
        remoteBasicProgressSummaryCardView.SetData(_dataSyncSelectPopupData.RemoteBasicProgressSummaryCardData);
        
        _selectLocalDataButton = localBasicProgressSummaryCardView.SelectButton;
        _selectRemoteDataButton = remoteBasicProgressSummaryCardView.SelectButton;

        _selectLocalDataButton.onClick.AddListener(() => OnSelectButtonClicked(localBasicProgressSummaryCardView.Data));
        _selectRemoteDataButton.onClick.AddListener(() => OnSelectButtonClicked(remoteBasicProgressSummaryCardView.Data));
        
        base.Show(null);
    }

    private void OnSelectButtonClicked(BasicProgressSummaryCardModel basicProgressSummaryCardData)
    {
        var criticalConfirmationPopupData = new CriticalConfirmationPopupData(default, default, OnConfirmCallback, OnRejectCallback);
        GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.CriticalConfirmationPopup, criticalConfirmationPopupData);
        
        void OnRejectCallback()
        {
            GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.CriticalConfirmationPopup);
        }

        void OnConfirmCallback()
        {
            GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.CriticalConfirmationPopup);
            
            _dataSyncSelectPopupData.OnDataSelected?.Invoke(basicProgressSummaryCardData.dataSourceType);
            
            GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.DataSyncSelectPopup);
        }
    }
    
    public override void Hide()
    {
        _selectLocalDataButton.onClick.RemoveListener(() => OnSelectButtonClicked(localBasicProgressSummaryCardView.Data));
        _selectRemoteDataButton.onClick.RemoveListener(() => OnSelectButtonClicked(remoteBasicProgressSummaryCardView.Data));
        
        base.OnHidden();
    }
}

public partial class UITypes
{
    public const string DataSyncSelectPopup = "DataSyncSelectPopup";
}

public abstract class BasePopupData : IBaseUIData
{
    public string header;
    public string message;

    protected BasePopupData(string header, string message)
    {
        this.header = header;
        this.message = message;
    }
}

public class DataSyncSelectPopupData : BasePopupData
{
    public Action<DataSourceType> OnDataSelected;
    
    public readonly BasicProgressSummaryCardModel RemoteBasicProgressSummaryCardData;
    public readonly BasicProgressSummaryCardModel LocalBasicProgressSummaryCardData;
    
    public DataSyncSelectPopupData(string header, string message, BasicProgressSummaryCardModel remoteBasicProgressSummaryCardData, BasicProgressSummaryCardModel localBasicProgressSummaryCardData, Action<DataSourceType> onDataSelected = null) : base(header, message)
    {
        OnDataSelected = onDataSelected;
        this.RemoteBasicProgressSummaryCardData = remoteBasicProgressSummaryCardData;
        this.LocalBasicProgressSummaryCardData = localBasicProgressSummaryCardData;
    }
}



