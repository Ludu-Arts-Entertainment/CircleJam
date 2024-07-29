using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBlocker : BlockerBase
{
    [SerializeField] private Animator handAnimator;
    [SerializeField] private Transform hand;
    [SerializeField] private Image handImage;
    [SerializeField] private GameObject maskCircle, maskBox, raycastBlocker;
    [SerializeField] private Sprite normalHand, clickHand;
    [SerializeField] private TextMeshProUGUI descriptiontext;

    private TutorialUIData _tutorialData;

    public override void Show(IBaseUIData data)
    {
        _tutorialData= data as TutorialUIData;
        Reset();

        #region TutorialType

        #region TapSection

        if (_tutorialData.tapData!=null)
        {
            hand.gameObject.SetActive(true);
            var tapData = _tutorialData.tapData;
            hand.position = tapData.posStart;
            if (tapData.animated)
                handAnimator.enabled = true;
            SetDescription(tapData.description);
        }


        #endregion

        #region SlideSection

        if (_tutorialData.slideData!=null)
        {
            var slideData = _tutorialData.slideData;
            Sequence seq = DOTween.Sequence();
            
            hand.gameObject.SetActive(true);
            hand.transform.position = slideData.posStart;
            hand.transform.localScale = Vector2.one * 1.2f;
            seq.Append(hand.transform.DOScale(Vector3.one , .2f).SetEase(Ease.Linear))
                .AppendInterval(.3f)
                .Append(hand.transform.DOMove(slideData.posEnd, .5f))
                .Append(handImage.DOFade(0, .5f)
                    .SetDelay(.5f)
                    .OnComplete(()=>handImage.color=new Color(255,255,255,1)))
                .SetLoops(-1).SetId("TutorialHand");
            
            seq.SetUpdate(true);
            if (slideData.animated)
                handAnimator.enabled = true;

            SetDescription(slideData.description);
        }


        #endregion
        
        #region MaskSecton

        if (_tutorialData.maskData!=null)
        {
            var maskData = _tutorialData.maskData;
            Transform tempMask = null;

            var xSize = 0f;
            var ySize = 0f;

            if (maskData.type == MaskType.Circle)
            {
                maskCircle.SetActive(true);
                tempMask = maskCircle.transform;
            }

            if (maskData.type == MaskType.Square)
            {
                maskBox.SetActive(true);
                tempMask = maskBox.transform;
            }

            xSize = maskData.size.x / 100f;
            ySize = maskData.size.y / 100f;
            tempMask.localScale = new Vector3(xSize, ySize, 1);

            tempMask.position = maskData.pos;
        }


        #endregion

        #region HideSection
        if(_tutorialData.hideData!=null)
        {
            hand.gameObject.SetActive(false);
            raycastBlocker.SetActive(true);
        }
        #endregion
       
        #endregion
        
        base.Show(data);
    }

    public override void Hide()
    {
        Reset();

        Debug.Log("TutorialPanel Hide");
        DOTween.Kill("TutorialHand");
        base.Hide();
    }

    public void SetDescription(string desc)
    {
        descriptiontext.text = desc;
    }

    public void Reset()
    {
        maskCircle.SetActive(false);
        maskBox.SetActive(false);

        handAnimator.enabled = false;
        handImage.color = Color.white;

        raycastBlocker.SetActive(false);
    }
}

public partial class UITypes
{
    public const string TutorialBlocker = nameof(TutorialBlocker);
}
