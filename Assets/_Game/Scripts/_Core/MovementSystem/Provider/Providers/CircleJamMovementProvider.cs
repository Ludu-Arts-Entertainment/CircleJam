using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleJamMovementProvider : IMovementProvider
{
    public IMovementProvider CreateSelf()
    {
        return new CircleJamMovementProvider();
    }

    public void Initialize(Action onReady)
    {
        GameInstaller.Instance.SystemLocator.InputManager.PointerDown += OnPointerDown;
        onReady?.Invoke();
    }

   private Vector3 initialDirection;
    private GridNode _selectedGridNode;
    private void OnPointerDown(object sender, PointerDownEventArgs e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e.ScreenPosition);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit);
        
        if(hasHit)
        {
            if(hit.collider.TryGetComponent(out GridNode gridNode))
            {
                _selectedGridNode = gridNode;
                initialDirection = (hit.point - gridNode.transform.position).normalized;

                Debug.Log("Selected Grid Node Level: " + _selectedGridNode.GridLevel);

                GameInstaller.Instance.SystemLocator.InputManager.PointerDrag += OnPointerDrag;
                GameInstaller.Instance.SystemLocator.InputManager.PointerUp += OnPointerUp;
            }
        }
    }

    private float angle = 0f;
    private void OnPointerDrag(object sender, PointerDragEventArgs e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e.ScreenPosition);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit);
        if(!hasHit) return;
        
        Vector3 currentDirection = (hit.point - _selectedGridNode.transform.position).normalized;
        angle = Vector3.SignedAngle(initialDirection, currentDirection, Vector3.up);

        GameInstaller.Instance.SystemLocator.GridManager.RotateCircle(_selectedGridNode.GridLevel, angle);

        initialDirection = currentDirection;
    }

    private void OnPointerUp(object sender, PointerUpEventArgs e)
    {
        GameInstaller.Instance.SystemLocator.GridManager.StopRotateCircle(_selectedGridNode.GridLevel);

        _selectedGridNode = null;

        GameInstaller.Instance.SystemLocator.InputManager.PointerDrag -= OnPointerDrag;
        GameInstaller.Instance.SystemLocator.InputManager.PointerUp -= OnPointerUp;
    }

}