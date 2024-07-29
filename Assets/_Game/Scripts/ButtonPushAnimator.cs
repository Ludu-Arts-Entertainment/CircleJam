using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPushAnimator : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField]private RectTransform textRectTransform;
    [SerializeField]private float animationDuration = 0.2f;
    [SerializeField]private float offset = -15;
    [SerializeField]private Sprite _pushSprite;
    private float _originalYPos;
    private Image _image;
    private Sprite _defaultSprite;
    private void Awake()
    {
        _originalYPos = textRectTransform.anchoredPosition.y;
        _image = GetComponent<Image>();
        _defaultSprite = _image.sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        textRectTransform.DOAnchorPosY(_originalYPos+offset, animationDuration);
        _image.sprite = _pushSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        textRectTransform.DOAnchorPosY(_originalYPos, animationDuration);
        _image.sprite = _defaultSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textRectTransform.DOAnchorPosY(_originalYPos, animationDuration);
        _image.sprite = _defaultSprite;
    }
}
