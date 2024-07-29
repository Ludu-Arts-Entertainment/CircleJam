using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class FriendInteractionButtonGroupController : MonoBehaviour
{
    public List<ButtonTypeButtonTuple> _buttonTypeButtonTuples;
    private FriendInfoModel _friendInfoModel;
    [EnumFlags,SerializeField]
    private FriendInfoModel.ButtonType _hideButtonTypes;
    private List<FriendInfoModel.ButtonType> _hideButtonTypeList = new List<FriendInfoModel.ButtonType>();
    
    public void SetData(FriendInfoModel friendInfoModel)
    {
        _friendInfoModel = friendInfoModel;
        _hideButtonTypeList = HideButtonsToList();
        SetButtons(friendInfoModel.Buttons.ToList());
    }
    private void SetButtons(List<FriendInfoModel.ButtonType> buttonTypes)
    {
        foreach (var buttonTypeButtonTuple in _buttonTypeButtonTuples)
        {
            bool isHide = _hideButtonTypeList.Contains(buttonTypeButtonTuple.buttonType);
            bool isShow = !isHide && buttonTypes.Contains(buttonTypeButtonTuple.buttonType);
            buttonTypeButtonTuple.button.gameObject.SetActive(isShow);
            if (isShow)
            {
                buttonTypeButtonTuple.button.SetData(_friendInfoModel);
            }
        }
    }
    private List<FriendInfoModel.ButtonType> HideButtonsToList()
    {
        return _hideButtonTypes.GetFlags().ToList();
    }
    [Serializable]
    public class ButtonTypeButtonTuple
    {
        public FriendInfoModel.ButtonType buttonType;
        public FriendInteractionButtonBase button;
    }
}