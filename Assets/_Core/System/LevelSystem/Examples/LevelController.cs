#if !LevelManager_Modified
using UnityEngine;

public class LevelController : MonoBehaviour, IPoolObject
{
    [SerializeField] private Transform gridParent;
    public string PoolId { get; set; }

    public void OnSpawned()
    {
        GameInstaller.Instance.SystemLocator.GridManager.CreateGrid(4, gridParent);
    }

    public void OnRecycled()
    {
        
    }
}
#endif

