#if !LevelManager_Modified
using UnityEngine;

public class LevelController : MonoBehaviour, IPoolObject
{
    public string PoolId { get; set; }

    public void OnSpawned()
    {
        
    }

    public void OnRecycled()
    {
        
    }
}
#endif

