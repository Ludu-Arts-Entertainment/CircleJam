using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CircleJamMovementProvider : IMovementProvider
{
    private int currentMoveCount;
    private int totalMoveCount;
    public IMovementProvider CreateSelf()
    {
        return new CircleJamMovementProvider();
    }

    public void Initialize(Action onReady)
    {
        totalMoveCount = 10;
        currentMoveCount = 10;

        GameInstaller.Instance.SystemLocator.InputManager.PointerDown += OnPointerDown;
        onReady?.Invoke();
    }

   private Vector3 initialDirection;
    private GridNode _selectedGridNode;
    private async void OnPointerDown(object sender, PointerDownEventArgs e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e.ScreenPosition);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit);

        totalAngle = 0;

        if(hasHit)
        {
            if(hit.collider.TryGetComponent(out GridNode gridNode))
            {
                _selectedGridNode = gridNode;
                initialDirection = (hit.point - gridNode.transform.position).normalized;

                await UniTask.Delay(20);
                GameInstaller.Instance.SystemLocator.InputManager.PointerDrag += OnPointerDrag;
                GameInstaller.Instance.SystemLocator.InputManager.PointerUp += OnPointerUp;
            }
        }
    }

    private float totalAngle;
    private void OnPointerDrag(object sender, PointerDragEventArgs e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e.ScreenPosition);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit);
        if(!hasHit) return;
        
        Vector3 currentDirection = (hit.point - _selectedGridNode.transform.position).normalized;
        var angle = Vector3.SignedAngle(initialDirection, currentDirection, Vector3.up);
        totalAngle += angle;
        GameInstaller.Instance.SystemLocator.GridManager.RotateCircle(_selectedGridNode.GridLevel, angle);

        initialDirection = currentDirection;
    }

    private void OnPointerUp(object sender, PointerUpEventArgs e)
    {
        GameInstaller.Instance.SystemLocator.GridManager.StopRotateCircle(_selectedGridNode.GridLevel);

        _selectedGridNode = null;

        if(Mathf.Abs(totalAngle) > 2)
        {
            currentMoveCount--;
            GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.MoveCountUpdated(currentMoveCount));
        }

        GameInstaller.Instance.SystemLocator.InputManager.PointerDrag -= OnPointerDrag;
        GameInstaller.Instance.SystemLocator.InputManager.PointerUp -= OnPointerUp;
    }
}

public partial class Events
{
    public class MoveCountUpdated : IEvent
    {
        public int CurrentMoveCount { get; }

        public MoveCountUpdated(int currentMoveCount)
        {
            CurrentMoveCount = currentMoveCount;
        }
    }
}