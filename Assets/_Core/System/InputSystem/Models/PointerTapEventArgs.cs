using System;
using UnityEngine;

public class PointerTapEventArgs : EventArgs
{
    public Vector2 ScreenPosition { get; set; }
    public Ray Ray { get; set; }
}
