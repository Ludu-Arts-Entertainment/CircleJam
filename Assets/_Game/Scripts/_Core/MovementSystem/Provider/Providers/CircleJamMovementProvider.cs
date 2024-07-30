using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CircleJamMovementProvider : IMovementProvider
{
    private int currentMoveCount;
    private int totalMoveCount;

    private bool _isLevelStopped = false;

    public IMovementProvider CreateSelf()
    {
        return new CircleJamMovementProvider();
    }

    public void Initialize(Action onReady)
    {
        GameInstaller.Instance.SystemLocator.InputManager.PointerDown += OnPointerDown;

        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnLevelLoaded>(OnLevelLoaded);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnLevelStopped>(OnLevelStopped);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.OnLevelContiuned>(OnLevelContinued);

        onReady?.Invoke();
    }

    private void OnLevelLoaded(Events.OnLevelLoaded loaded)
    {
        //Sonrasında datadan alınacak
        totalMoveCount = 10;
        currentMoveCount = 10;
        _isLevelStopped = false;
    }

     private void OnLevelContinued(Events.OnLevelContiuned contiuned)
    {
        _isLevelStopped = false;
    }

    private void OnLevelStopped(Events.OnLevelStopped stopped)
    {
        _isLevelStopped = true;
    }

   private Vector3 initialDirection;
    private GridNode _selectedGridNode;
    private async void OnPointerDown(object sender, PointerDownEventArgs e)
    {
        if(_isLevelStopped) return;
        
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
            CheckFail();
        }

        GameInstaller.Instance.SystemLocator.InputManager.PointerDrag -= OnPointerDrag;
        GameInstaller.Instance.SystemLocator.InputManager.PointerUp -= OnPointerUp;
    }

    private void CheckFail()
    {
        if(currentMoveCount <= 0)
        {
            GameInstaller.Instance.SystemLocator.UIManager.Show(UITypes.FailPopup, null);
        }
    }
}


public partial class Events
{
    public struct MoveCountUpdated : IEvent
    {
        public int CurrentMoveCount { get; }

        public MoveCountUpdated(int currentMoveCount)
        {
            CurrentMoveCount = currentMoveCount;
        }
    }
}