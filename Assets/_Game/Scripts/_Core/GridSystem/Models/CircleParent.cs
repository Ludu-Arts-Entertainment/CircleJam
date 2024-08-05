using System.Collections.Generic;
using UnityEngine;

public class CircleParent : MonoBehaviour
{
    public List<Circle> CircleParents => circleParents;
    [SerializeField] private List<Circle> circleParents;

    public Transform DoorTransform => doorTransform;
    [SerializeField] private Transform doorTransform;

}