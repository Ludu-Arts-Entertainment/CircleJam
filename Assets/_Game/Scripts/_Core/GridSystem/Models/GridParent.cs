using System.Collections.Generic;
using UnityEngine;

public class GridParent : MonoBehaviour
{
    public List<Transform> GridCircleParents => gridCircleParents;
    [SerializeField] private List<Transform> gridCircleParents;

    public Transform DoorTransform => doorTransform;
    [SerializeField] private Transform doorTransform;

}
