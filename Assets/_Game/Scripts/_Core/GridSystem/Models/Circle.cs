using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private Transform rotateTransform, notRotateTransform;
    public Transform RotateTransform => rotateTransform;
    public Transform NotRotateTransform => notRotateTransform;
}
