using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CriticalConfirmationPopup : PopupBase
{
    private const string Header = "Are you sure?";
    private const string Message = "Your current data\nreplaced with\n selected one!";
    
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _message;
    
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TextMeshProUGUI _confirmButtonText;
    [SerializeField] private Button _rejectButton;
    [SerializeField] private TextMeshProUGUI _rejectButtonText;
    
    private CriticalConfirmationPopupData _criticalConfirmationPopupData;
    
    public override void Show(IBaseUIData data)
    {
        if (data is not CriticalConfirmationPopupData criticalConfirmationPopupData)
            return;
        
        // cache DataSyncSelectPopupData on a variable
        _criticalConfirmationPopupData = criticalConfirmationPopupData;

        _header.text = String.IsNullOrEmpty(_criticalConfirmationPopupData.header) ? _criticalConfirmationPopupData.header : Header;
        _message.text = String.IsNullOrEmpty(_criticalConfirmationPopupData.message) ? _criticalConfirmationPopupData.message : Message;

        _confirmButtonText.text ??= "Confirm";
        _rejectButtonText.text ??= "Reject";
        
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        _rejectButton.onClick.AddListener(OnRejectButtonClicked);
        
        base.Show(null);
    }

    private void OnConfirmButtonClicked()
    {
        _criticalConfirmationPopupData.OnConfirm?.Invoke();
    }
    
    private void OnRejectButtonClicked()
    {
        _criticalConfirmationPopupData.OnReject?.Invoke();
    }

    public override void Hide()
    {
        _confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
        _rejectButton.onClick.RemoveListener(OnRejectButtonClicked);
        
        base.OnHidden();
    }
}

public class CriticalConfirmationPopupData : BasePopupData
{
    public Action OnConfirm;
    public Action OnReject;
    public CriticalConfirmationPopupData(string header, string message, Action onConfirm, Action onReject) : base(header, message)
    {
        OnConfirm = onConfirm;
        OnReject = onReject;
    }
}

public partial class UITypes
{
    public const string CriticalConfirmationPopup = "CriticalConfirmationPopup";
}