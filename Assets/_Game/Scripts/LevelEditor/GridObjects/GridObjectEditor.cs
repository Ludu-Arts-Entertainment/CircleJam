using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GridObjectEditor : SerializedMonoBehaviour
{
    public virtual void Remove()
    {
        DestroyImmediate(gameObject);
    }
}
