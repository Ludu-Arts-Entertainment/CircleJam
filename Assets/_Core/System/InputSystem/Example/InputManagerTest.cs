using UnityEngine;

public class InputManagerTest : MonoBehaviour
{
    private void Start()
    {
        GameInstaller.Instance.SystemLocator.InputManager.PointerTap += OnPointerTap;
        GameInstaller.Instance.SystemLocator.InputManager.PointerDown += OnPointerDown;
        GameInstaller.Instance.SystemLocator.InputManager.PointerUp += OnPointerUp;
        GameInstaller.Instance.SystemLocator.InputManager.PointerSwipe += OnPointerSwipe;
    }
    private void OnPinch(object sender, PointerPinchEventArgs e)
    {
        Debug.Log($"[Input Manager Test] OnPinch \nParams: PinchRatio: {e.PinchRatio}");
    }

    private void OnPointerSwipe(object sender, PointerSwipeDetectEventArgs e)
    {
        Debug.Log($"[Input Manager Test] OnPointerSwipe \nParams: ScreenPosition: {e.ScreenPosition} \tDirection: {e.Direction}\tMagnitude: {e.Magnitude} \tFourDirection: {e.FourDirection} \tEightDirection: {e.EightDirection}");
    }

    private void OnPointerDrag(object sender, PointerDragEventArgs e)
    {
        if (e.DragObject is not null)
        {
            // Store old position
            var oldPosition = transform.position;

            // Screen position of the transform
            var screenPosition = Camera.main.WorldToScreenPoint(oldPosition);

            // Add the deltaPosition
            screenPosition += (Vector3)e.ScreenDelta;

            // Convert back to world space
            var newPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // Add to delta
            e.DragObject.transform.localPosition += newPosition - oldPosition;
        }
        Debug.Log($"[Input Manager Test] OnPointerDrag \nParams: ScreenPosition: {e.ScreenPosition} \tTarget: {e.DragObject} ");
    }

    private void OnPointerUp(object sender, PointerUpEventArgs e)
    {
        GameInstaller.Instance.SystemLocator.InputManager.PointerDrag -= OnPointerDrag;
        GameInstaller.Instance.SystemLocator.InputManager.OnPinch -= OnPinch;
        Debug.Log($"[Input Manager Test] OnPointerUp \nParams: ScreenPosition: {e.ScreenPosition} ");
    }

    private void OnPointerDown(object sender, PointerDownEventArgs e)
    {
        GameInstaller.Instance.SystemLocator.InputManager.PointerDrag += OnPointerDrag;
        GameInstaller.Instance.SystemLocator.InputManager.OnPinch += OnPinch;
        Debug.Log($"[Input Manager Test] OnPointerDown \nParams: ScreenPosition: {e.ScreenPosition} \tTarget: {e.Target}");
    }

    private void OnPointerTap(object sender, PointerTapEventArgs e)
    {
        Debug.Log($"[Input Manager Test] OnPointerTap \nParams: ScreenPosition: {e.ScreenPosition} ");
    }
}
