using System;
using UnityEngine;

public class InputManager : IManager
{
    private IInputProvider _inputProvider;
    public event EventHandler<PointerDownEventArgs> PointerDown;
    public event EventHandler<PointerUpEventArgs> PointerUp;
    public event EventHandler<PointerDragEventArgs> PointerDrag;
    public event EventHandler<PointerSwipeDetectEventArgs> PointerSwipe;
    public event EventHandler<PointerTapEventArgs> PointerTap;
    public event EventHandler<PointerPinchEventArgs> OnPinch;
    public IManager CreateSelf()
    {
        return new InputManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _inputProvider = InputProviderFactory.Create(InputProviderEnums.LeanTouchInputProvider);
        _inputProvider.Initialize(() =>
        {
            _inputProvider.PointerTap += (sender, args) => PointerTap?.Invoke(this, args);
            _inputProvider.PointerDown += (sender, args) => PointerDown?.Invoke(this, args);
            _inputProvider.PointerUp += (sender, args) => PointerUp?.Invoke(this, args);
            _inputProvider.PointerDrag += (sender, args) => PointerDrag?.Invoke(this, args);
            _inputProvider.PointerSwipe += (sender, args) => PointerSwipe?.Invoke(this, args);
            _inputProvider.OnPinch += (sender, args) => OnPinch?.Invoke(this, args);
            onReady?.Invoke();
        });
    }

    public bool IsReady()
    {
        return _inputProvider != null;
    }
    public void Close()
    {
        Debug.Log("Input Closed");
        _inputProvider.Close();
    }
    public void Open()
    {
        Debug.Log("Input Opened");
        _inputProvider.Open();
    }
}