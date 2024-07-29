
using System;
using UnityEngine;

public class PointerDownEventArgs : EventArgs
{
    public Vector2 ScreenPosition { get; set; }
    public Ray Ray { get; set; }
    public object Target { get; set; }
    public object Target2D { get; set; }
}
