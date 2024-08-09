using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectEditor : MonoBehaviour
{
   public void Remove()
    {
        DestroyImmediate(gameObject);
    }
}
