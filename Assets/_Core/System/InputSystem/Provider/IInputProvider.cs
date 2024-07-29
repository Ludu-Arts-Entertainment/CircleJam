using System;

public interface IInputProvider
{
    IInputProvider CreateSelf();
    void Initialize(Action onReady);
    public event EventHandler<PointerDownEventArgs> PointerDown;
    public event EventHandler<PointerUpEventArgs> PointerUp;
    public event EventHandler<PointerDragEventArgs> PointerDrag;
    public event EventHandler<PointerSwipeDetectEventArgs> PointerSwipe;
    public event EventHandler<PointerTapEventArgs> PointerTap;
    public event EventHandler<PointerPinchEventArgs> OnPinch;
    void Close();
    void Open();
}
