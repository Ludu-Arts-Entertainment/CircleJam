using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridParent : MonoBehaviour
{
    public List<Transform> GridCircleParents => gridCircleParents;
    [SerializeField] private List<Transform> gridCircleParents;

}
