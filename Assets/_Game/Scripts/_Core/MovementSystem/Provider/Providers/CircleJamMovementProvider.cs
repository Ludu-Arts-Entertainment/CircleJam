using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CircleJamMovementProvider : IMovementProvider
{
    public int MovementCount => currentMoveCount;
    private int currentMoveCount;

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
        currentMoveCount = 10;

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.MoveCountUpdated(currentMoveCount));
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
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);

        totalAngle = 0;

        if(hasHit)
        {
            if(hit.collider.TryGetComponent(out GridNode gridNode) && gridNode.IsCanRotate())
            {
                _selectedGridNode = gridNode;
            }
            else if(hit.collider.TryGetComponent(out CharacterController character))
            {
                _selectedGridNode = character.CurrentGridNode;
            }

            if(_selectedGridNode == null) return;

            Physics.Raycast(ray, out RaycastHit hitPoint, Mathf.Infinity, LayerMask.GetMask("Ground"));
            initialDirection = (hitPoint.point - _selectedGridNode.transform.position).normalized;
            GameInstaller.Instance.SystemLocator.GridManager.StartRotateCircle(_selectedGridNode.GridNodeData.CircleLevel);

            await UniTask.Delay(20);

            GameInstaller.Instance.SystemLocator.InputManager.PointerDrag += OnPointerDrag;
            GameInstaller.Instance.SystemLocator.InputManager.PointerUp += OnPointerUp;

            GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.DragStarted());
        }
    }

    private float totalAngle;
    private void OnPointerDrag(object sender, PointerDragEventArgs e)
    {
        Ray ray = Camera.main.ScreenPointToRay(e.ScreenPosition);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hitPoint, Mathf.Infinity, LayerMask.GetMask("Ground"));

        if(!hasHit) return;

        Vector3 currentDirection = (hitPoint.point - _selectedGridNode.transform.position).normalized;
        var angle = Vector3.SignedAngle(initialDirection, currentDirection, Vector3.up);
        totalAngle += angle;
        GameInstaller.Instance.SystemLocator.GridManager.RotateCircle(_selectedGridNode.GridNodeData.CircleLevel, angle);

        initialDirection = currentDirection;
    }

    private void OnPointerUp(object sender, PointerUpEventArgs e)
    {
        GameInstaller.Instance.SystemLocator.GridManager.StopRotateCircle(_selectedGridNode.GridNodeData.CircleLevel, totalAngle);

        _selectedGridNode = null;

        if(Mathf.Abs(totalAngle) > 2)
        {
            currentMoveCount--;
            GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.MoveCountUpdated(currentMoveCount));
            CheckFail();
        }

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.DragStopped());

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

    public struct DragStarted : IEvent
    {
    }

    public struct DragStopped : IEvent
    {
    }
}