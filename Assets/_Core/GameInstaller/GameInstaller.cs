using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    public static GameInstaller Instance;
    [HideInInspector]public SystemLocator SystemLocator;
    public List<ManagerEnums> Managers;
    public Customizer Customizer;
    public Dictionary<ManagerEnums, IManager> ManagerDictionary = new Dictionary<ManagerEnums, IManager>();
    private readonly List<ManagerEnums> _initializingManagers = new List<ManagerEnums>();
    public List<ProductBlockTypeGiverTuple> Givers;
    public List<PriceTypeGiverTuple> Payers;
    public List<RequirementTypeRequirementTuple> Requirements;
    
    private async void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        SystemLocator = new SystemLocator();
        var eventManager = ManagerFactory.Create(ManagerEnums.EventManager);
        eventManager.Initialize(this, () =>
        {
            ManagerDictionary.Add(ManagerEnums.EventManager, eventManager);
        });
        await UniTask.WaitUntil(eventManager.IsReady);
        Initialize();
    }
    private void Initialize()
    {
        foreach (var managerEnum in Managers)
        {
            var manager = ManagerFactory.Create(managerEnum);
            _initializingManagers.Add(managerEnum);
            manager.Initialize(this, () =>
            {
                _initializingManagers.Remove(managerEnum);
                ManagerDictionary.Add(managerEnum,manager);
            });
        }
        StartCoroutine(CheckIsAllManagersReady());
    }
    private void Starter()
    {
        Debug.Log("All Managers are ready!\nInitialized Managers:\n\t" + string.Join("\n\t", ManagerDictionary.Keys));
        SystemLocator.EventManager.Trigger(new Events.OnGameReadyToStart());
    }
    private IEnumerator CheckIsAllManagersReady()
    {
        yield return new WaitUntil(() => _initializingManagers.Count == 0);
        Starter();
    }
}
