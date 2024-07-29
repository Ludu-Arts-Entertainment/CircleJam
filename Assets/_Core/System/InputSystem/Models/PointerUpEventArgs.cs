using System;
using UnityEngine;

public class PointerUpEventArgs : EventArgs
{
    public Vector2 ScreenPosition { get; set; }
    public Ray Ray { get; set; }

}
