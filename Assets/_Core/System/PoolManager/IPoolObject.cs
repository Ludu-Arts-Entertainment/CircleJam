using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{    
    string PoolId { get; set; }
    void OnSpawned();
    void OnRecycled();
}
