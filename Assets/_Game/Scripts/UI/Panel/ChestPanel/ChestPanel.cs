using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChestPanel : PanelBase
{
    private ChestPanelArgs _args;
    [SerializeField] private BasicChestProductElement[] chestProductElementList;
    private Queue<BasicChestProductElement> _chestProductElementQueue;
    [SerializeField] private Button claimButton;
    private List<ProductBlock> _productBlocks;
    [SerializeField] private BasicChestAnimationController basicChestAnimationController;
    private List<Vector2> _chestProductElementStartPositions;
    [SerializeField] private TMP_Text chestCountText;
    private readonly List<Sequence> _activeSequenceList=new ();
    private int currentChestCount;
    public override void Show(IBaseUIData data)
    {
        if (_chestProductElementStartPositions==null)
        {
            _chestProductElementStartPositions = new List<Vector2>();
            foreach (var pos in chestProductElementList)
            {
                _chestProductElementStartPositions.Add(pos.transform.position);
            }
        }
        _args = (ChestPanelArgs) data;
        currentChestCount = _args.ChestCount;
        UpdateChestCount(0);
        claimButton.gameObject.SetActive(false);
        base.Show(data);
    }

    private void UpdateChestCount(int amount)
    {
        currentChestCount += amount;
        chestCountText.text = currentChestCount.ToString();
    }
    protected override void OnShown()
    {
        basicChestAnimationController.gameObject.SetActive(true);
        basicChestAnimationController.PlayIdleExcited();
        GameInstaller.Instance.SystemLocator.InputManager.PointerTap += OpenChest;
        base.OnShown();
    }

    protected override void OnHidden()
    {
        Reset();
        base.OnHidden();
    }

    private void OpenChest(object sender, PointerTapEventArgs e)
    {
        Reset();
        _productBlocks = GameInstaller.Instance.SystemLocator.ChestManager.Open(_args.ChestType);
        UpdateChestCount(-1);
        PlaceItem();
    }
    private void PlaceItem()
    {
        if (_productBlocks.Count == 0)
        {
            return;
        }
        GameInstaller.Instance.SystemLocator.InputManager.PointerTap -= OpenChest;
        basicChestAnimationController.PlayOpen();
        int i = 0;
        foreach (var productBlock in _productBlocks)
        {
            _chestProductElementQueue.TryDequeue(out var chestProductElement);
            
            var targetPos= chestProductElement.transform.position;
            Sequence sequence = DOTween.Sequence();
            _activeSequenceList.Add(sequence);
            sequence.SetDelay(.3f+i*.1f);
            sequence.Append(chestProductElement.transform.DOScale(.2f, 0f)
                    .OnComplete(() => chestProductElement.Set(productBlock)))
                .Join(chestProductElement.transform.DOMove(basicChestAnimationController.transform.position, 0f))
                .Append(chestProductElement.transform.DOMoveX(targetPos.x, .5f).SetEase(Ease.Unset))
                .Join(chestProductElement.transform.DOMoveY(targetPos.y, .5f).SetEase(Ease.OutCirc))
                .Join(chestProductElement.transform.DOScale(1, .4f)).OnComplete(() =>
                {
                    _activeSequenceList.Remove(sequence);
                });
            i++;
        }
        GameInstaller.Instance.SystemLocator.InputManager.PointerTap += SkipAnimation;
        CoroutineController.DoAfterCondition(()=>_activeSequenceList.Count==0, () =>
        {
            GameInstaller.Instance.SystemLocator.InputManager.PointerTap -= SkipAnimation;
            if (currentChestCount>0)
            {
                GameInstaller.Instance.SystemLocator.InputManager.PointerTap += OpenChest;
            }
            else
            {
                claimButton.onClick.AddListener(OnClaimClicked);
                claimButton.gameObject.SetActive(true);
            }
        });
    }

    private void SkipAnimation(object sender, PointerTapEventArgs e)
    {
        var anims = new List<Sequence>(_activeSequenceList);
        foreach (var animation in anims)
        {
            if (!animation.IsComplete())
            {
                animation.Complete();
            }
        }
    }

    public void Reset()
    {
        var chestData = GameInstaller.Instance.SystemLocator.ChestManager.GetChestData(_args.ChestType);
        switch (chestData.WillGiveCount)
        {
            case 1:
                _chestProductElementQueue = new Queue<BasicChestProductElement>();
                _chestProductElementQueue.Enqueue(chestProductElementList[5]);
                break;
            case 2:
                _chestProductElementQueue = new Queue<BasicChestProductElement>();
                _chestProductElementQueue.Enqueue(chestProductElementList[3]);
                _chestProductElementQueue.Enqueue(chestProductElementList[4]);
                break;
            case 3:
                _chestProductElementQueue = new Queue<BasicChestProductElement>();
                _chestProductElementQueue.Enqueue(chestProductElementList[3]);
                _chestProductElementQueue.Enqueue(chestProductElementList[4]);
                _chestProductElementQueue.Enqueue(chestProductElementList[5]);
                break;
            default:
                _chestProductElementQueue = new Queue<BasicChestProductElement>(chestProductElementList);
                break;
        }
        int queue = 0;
        foreach (var chestProductElement in chestProductElementList)
        {
            chestProductElement.transform.position = _chestProductElementStartPositions[queue];
            chestProductElement.Reset();
            queue++;
        }

        foreach (var sequence in _activeSequenceList)
        {
            DOTween.Kill(sequence);
        }
    }
    private void OnClaimClicked()
    {
        claimButton.onClick.RemoveListener(OnClaimClicked);
        GameInstaller.Instance.SystemLocator.UIManager.Hide(UITypes.ChestPanel);
    }
}
public struct ChestPanelArgs : IBaseUIData
{
    public string ChestType;
    public int ChestCount;
    
    public ChestPanelArgs(string chestType, int chestCount)
    {
        ChestType = chestType;
        ChestCount = chestCount;
    }
}
public partial class UITypes
{
    public const string ChestPanel = "ChestPanel";
}