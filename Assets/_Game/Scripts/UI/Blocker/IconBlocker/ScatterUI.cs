using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScatterUI : MonoBehaviour
{
    [SerializeField] private string scatterUIType;
    [SerializeField] private Image image;
    [SerializeField] private Ease randomCircleEase = Ease.OutQuad;
    [SerializeField] private Ease moveEase = Ease.Linear;
    [SerializeField] private float moveTime = 0.5f;
    [SerializeField] private float randomCircleTime = 0.2f;
    [SerializeField] private float extraWaitTimeForEnd = 0.5f;
    [SerializeField] private Vector2 randomWaitRange = new(0.1f, 0.3f);
    
    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
    public void StartAnimation(float randomCircleRange, float moveScale, Vector3 endPoint, Action endActio)
    {
        StartCoroutine(ScatterUIAnimation(randomCircleRange, moveScale, endPoint, endActio));
    }
    
    public float GetTotalAnimationTime()
    {
        return randomCircleTime + moveTime + randomWaitRange.y + extraWaitTimeForEnd;
    }

    private IEnumerator ScatterUIAnimation(float randomCircleRange, float moveScale, Vector3 endPoint, Action endAction)
    {
        var randomCircle = UnityEngine.Random.insideUnitCircle * randomCircleRange;
        var randomPoint = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);

        transform.DOMove(randomPoint, randomCircleTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(randomCircleTime);

        yield return new WaitForSeconds(UnityEngine.Random.Range(randomWaitRange.x, randomWaitRange.y));
        transform.DOScale(moveScale, moveTime).SetEase(moveEase);

        transform.DOMove(endPoint, moveTime).SetEase(moveEase).OnComplete(()=>
        {
            endAction?.Invoke();
            GameInstaller.Instance.SystemLocator.PoolManager.Destroy(scatterUIType, this);
        });
        
    }
}
