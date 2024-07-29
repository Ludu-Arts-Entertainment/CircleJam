using System;
using UnityEngine;

public class PointerDragEventArgs : EventArgs
{
    public Vector2 ScreenPosition { get; set; }
    public Ray Ray { get; set; }
    public GameObject DragObject { get; set; }
    public Vector2 ScreenDelta { get; set; }
    public GameObject DragObject2D { get; set; }
}
