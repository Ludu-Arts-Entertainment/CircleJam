using System;
using Lean.Touch;
using UnityEngine;

public class LeanTouchInputProvider : IInputProvider
{
    Camera _camera;
    public IInputProvider CreateSelf()
    {
        return new LeanTouchInputProvider();
    }

    public void Initialize(Action onReady)
    {
        _camera = Camera.main;
        LeanTouch.OnFingerTap += (finger) => PointerTap?.Invoke(this, new PointerTapEventArgs()
        {
            ScreenPosition = finger.ScreenPosition, 
            Ray = _camera.ScreenPointToRay(finger.ScreenPosition)
        });
        LeanTouch.OnFingerDown += finger => PointerDown?.Invoke(this, new PointerDownEventArgs()
        {
            ScreenPosition = finger.ScreenPosition,
            Ray = _camera.ScreenPointToRay(finger.ScreenPosition),
            Target = Physics.Raycast(_camera.ScreenPointToRay(finger.ScreenPosition), out var hit) ? hit.collider.gameObject : null,
            Target2D = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(finger.ScreenPosition)).collider?.gameObject
        });
        LeanTouch.OnFingerUp += finger => PointerUp?.Invoke(this, new PointerUpEventArgs()
        {
            ScreenPosition = finger.ScreenPosition,
            Ray = _camera.ScreenPointToRay(finger.ScreenPosition)
        });
        LeanTouch.OnFingerSwipe += finger => PointerSwipe?.Invoke(this, new PointerSwipeDetectEventArgs()
        {
            ScreenPosition = finger.ScreenPosition,
            Direction = finger.SwipeScreenDelta.normalized,
            Magnitude = finger.SwipeScreenDelta.magnitude,
        });
        LeanTouch.OnFingerUpdate += finger => PointerDrag?.Invoke(this, new PointerDragEventArgs()
        {
            ScreenPosition = finger.ScreenPosition,
            Ray = _camera.ScreenPointToRay(finger.ScreenPosition),
            DragObject = Physics.Raycast(_camera.ScreenPointToRay(finger.ScreenPosition), out var hit) ? hit.collider.gameObject : null,
            DragObject2D = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(finger.ScreenPosition)).collider?.gameObject ,
            ScreenDelta = finger.ScreenDelta
        });
        LeanTouch.OnGesture += (gesture) =>
        {
            var pinchScale = LeanGesture.GetPinchScale(gesture, 0);
            if (gesture.Count>=2 && (pinchScale is not (0 or 1)))
            {
                OnPinch?.Invoke(this, new PointerPinchEventArgs()
                {
                    PinchRatio = LeanGesture.GetPinchScale(gesture, 0)
                });
            }
        };
        onReady?.Invoke();
    }

    public event EventHandler<PointerDownEventArgs> PointerDown;
    public event EventHandler<PointerUpEventArgs> PointerUp;
    public event EventHandler<PointerDragEventArgs> PointerDrag;
    public event EventHandler<PointerSwipeDetectEventArgs> PointerSwipe;
    public event EventHandler<PointerTapEventArgs> PointerTap;
    public event EventHandler<PointerPinchEventArgs> OnPinch;
    public void Open()
    {
        LeanTouch.Instance.UseMouse = true;
        LeanTouch.Instance.UseTouch = true;
        LeanTouch.Instance.UseHover = true;
        LeanTouch.Instance.UseSimulator = true;
    }
    public void Close()
    {
        LeanTouch.Instance.UseMouse = false;
        LeanTouch.Instance.UseTouch = false;
        LeanTouch.Instance.UseHover = false;
        LeanTouch.Instance.UseSimulator = false;
    }
}