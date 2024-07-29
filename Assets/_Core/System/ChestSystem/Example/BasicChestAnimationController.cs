using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class BasicChestAnimationController : MonoBehaviour
{
    [FormerlySerializedAs("skeletonAnimation")] [SerializeField]
    private SkeletonGraphic skeletonGraphic;

    private TrackEntry _trackEntry;
    private string nextAnim;

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
    
    private void PlayAnimation(string anim, string nextAnim, bool isLoop = false)
    {
        this.nextAnim = nextAnim;
        if (!isLoop)
        {
            _trackEntry = skeletonGraphic.AnimationState.SetAnimation(0, anim, false);
            _trackEntry.Complete += OnCompleteTrack;
        }
        else
        {
            _trackEntry = skeletonGraphic.AnimationState.SetAnimation(0, anim, true);
        }
    }

    private void OnCompleteTrack(TrackEntry trackentry)
    {
        // _trackEntry = skeletonGraphic.AnimationState.SetAnimation(0, nextAnim, true);
        PlayIdleOpen();
    }

    public void PlayIdleCalm()
    {
        PlayAnimation("CHEST_IDLE_CALM", null, true);
    }

    public void PlayIdleExcited()
    {
        
        PlayAnimation("CHEST_IDLE_EXCITED", null, true);
    }

    public void PlayOpen()
    {
        PlayAnimation("CHEST_OPEN", "CHEST_OPEN_IDLE", false);
    }

    public void PlayIdleOpen()
    {
        PlayAnimation("CHEST_OPEN_IDLE", null, true);
    }
}