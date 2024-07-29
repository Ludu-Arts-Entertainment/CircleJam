using System;
using UnityEngine;

public class PointerSwipeDetectEventArgs : EventArgs
{
    public Vector2 Direction { get; set; }
    public float Magnitude { get; set; }
    public Vector2 ScreenPosition { get; set; }

    public SwipeDirection FourDirection
    {
        get
        {
            var normal = Direction.normalized;
            var tNormal = new Vector2(Mathf.Abs(normal.x), Mathf.Abs(normal.y));
            return normal switch
            {
                _ when tNormal.y>=tNormal.x && normal.y>0 => SwipeDirection.Up,
                _ when tNormal.y>=tNormal.x && normal.y<0 => SwipeDirection.Down,
                _ when tNormal.x>=tNormal.y && normal.x>0 => SwipeDirection.Right,
                _ when tNormal.x>=tNormal.y && normal.x<0 => SwipeDirection.Left,
                _ => SwipeDirection.None
            };
        }
    }

    public SwipeDirection EightDirection
    {
        get
        {
            var normal = Direction.normalized;
            var roundNormal = new Vector2((int)Math.Round(normal.x), (int)Math.Round(normal.y));
            return roundNormal switch
            {
                _ when roundNormal == GetCardinalDirections.Up => SwipeDirection.Up,
                _ when roundNormal == GetCardinalDirections.Down => SwipeDirection.Down,
                _ when roundNormal == GetCardinalDirections.Right => SwipeDirection.Right,
                _ when roundNormal == GetCardinalDirections.Left => SwipeDirection.Left,
                _ when roundNormal == GetCardinalDirections.UpRight => SwipeDirection.UpRight,
                _ when roundNormal == GetCardinalDirections.UpLeft => SwipeDirection.UpLeft,
                _ when roundNormal == GetCardinalDirections.DownRight => SwipeDirection.DownRight,
                _ when roundNormal == GetCardinalDirections.DownLeft => SwipeDirection.DownLeft,
                _ => SwipeDirection.None
            };
        }
    }
    private class GetCardinalDirections
    {
        public static readonly Vector2 Up = new Vector2(0, 1);
        public static readonly Vector2 Down = new Vector2(0, -1);
        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Left = new Vector2(-1, 0);

        public static readonly Vector2 UpRight = new Vector2(1, 1);
        public static readonly Vector2 UpLeft = new Vector2(-1, 1);
        public static readonly Vector2 DownRight = new Vector2(1, -1);
        public static readonly Vector2 DownLeft = new Vector2(-1, -1);
    }
}


public enum SwipeDirection
{
    None,
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    DownLeft,
    UpRight,
    DownRight
}