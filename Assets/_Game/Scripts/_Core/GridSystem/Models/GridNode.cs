using System;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private Color normalEmisionColor, selectedEmisionColor;

    public CharacterController Character => character;
    private CharacterController character;

    private MaterialPropertyBlock PropertyBlock => propertyBlock??= new MaterialPropertyBlock();
    private MaterialPropertyBlock propertyBlock;
    
    public GridNodeData GridNodeData => _gridNodeData;
    private GridNodeData _gridNodeData;
    private int GridIdx;

    private MeshRenderer meshRenderer;
    private Transform model, model2;
    public void Initialize(CircleData circleData, GridNodeData gridNodeData)
    {
        character = null;
        _gridNodeData = gridNodeData;
        GridIdx = _gridNodeData.GridIdx;

        if(_gridNodeData.GridType == GridType.Normal)
        {
            model = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>($"GridModel_{gridNodeData.CircleLevel + 1}", parent: transform);
            model.localPosition = Vector3.zero;
            model.localScale = Vector3.one;
            meshRenderer = model.GetComponentInChildren<MeshRenderer>();
        }
        if(_gridNodeData.GridType == GridType.FixedPath)
        {
            var modelName = GridNodeCollectionService.GetModelNameByFixedPathType(_gridNodeData.FixedPathType);
            model = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>(modelName, parent: transform);
            model.transform.localPosition = collider.bounds.center;
            model.transform.localPosition = new Vector3(model.transform.localPosition.x, 0.5f, model.transform.localPosition.z);
        }
        if(_gridNodeData.GridType == GridType.InteractablePath)
        {
            var modelName = GridNodeCollectionService.GetModelNameByInteractablePathType(_gridNodeData.InteractablePathType);
            model = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>(modelName, parent: transform);
            model.transform.localPosition = Vector3.zero;
        }
        if(_gridNodeData.GridType == GridType.FixedObstacle)
        {
            model = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>($"GridModel_{gridNodeData.CircleLevel + 1}", parent: transform);
            model.localPosition = Vector3.zero;
            model.localScale = Vector3.one;
            meshRenderer = model.GetComponentInChildren<MeshRenderer>();

            var model2Name = GridNodeCollectionService.GetModelNameByFixedObstacleType(_gridNodeData.FixedObstacleType);
            model2 = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>($"{model2Name}_{gridNodeData.CircleLevel + 1}", parent: circleData.Circle.NotRotateTransform);
            model2.rotation = transform.rotation;
            model2.localPosition = Vector3.zero;
        }
    }

    public void UpdateGridIdx(int gridIdx)
    {
        _gridNodeData.GridIdx = gridIdx;
        GridIdx = gridIdx;
    }

    public void CreateCharacter(GoalColor color, Transform lookTarget)
    {
        character = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<CharacterController>("Character", parent: transform);
        character.Initialize(color, this);
        character.transform.localPosition = collider.bounds.center;
        character.transform.localPosition = new Vector3(character.transform.localPosition.x, 0.5f, character.transform.localPosition.z);

        Vector3 direction = lookTarget.position - character.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        character.transform.rotation = Quaternion.Euler(0, angle, 0);

        _gridNodeData.HaveCharacter = true;

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.CharacterCreated(color, character));
    }

    public void SetSelectedColor(bool isSelected)
    {
        if(meshRenderer == null) return;

        meshRenderer.GetPropertyBlock(PropertyBlock);
        PropertyBlock.SetColor("_EmissionColor", isSelected ? selectedEmisionColor : normalEmisionColor);
        meshRenderer.SetPropertyBlock(PropertyBlock);
    }

    public void ResetGrid()
    {
        if(_gridNodeData != null)
        {
            if(_gridNodeData.GridType == GridType.Normal)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy($"GridModel_{_gridNodeData.CircleLevel + 1}", model);
            }
            else if(_gridNodeData.GridType == GridType.FixedPath)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy(GridNodeCollectionService.GetModelNameByFixedPathType(_gridNodeData.FixedPathType), model);
            }
            else if(_gridNodeData.GridType == GridType.InteractablePath)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy(GridNodeCollectionService.GetModelNameByInteractablePathType(_gridNodeData.InteractablePathType), model);
            }
            else if(_gridNodeData.GridType == GridType.FixedObstacle)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy($"GridModel_{_gridNodeData.CircleLevel + 1}", model);
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy($"{GridNodeCollectionService.GetModelNameByFixedObstacleType(_gridNodeData.FixedObstacleType)}_{_gridNodeData.CircleLevel + 1}", model2);
            }
        }
    }

    public bool IsCanRotate()
    {
        return _gridNodeData.GridType != GridType.FixedPath && _gridNodeData.GridType != GridType.Empty;
    }
}

public class GridNodeData
{
    public GridType GridType;
    public FixedPathType FixedPathType;
    public InteractablePathType InteractablePathType;
    public FixedObstacleType FixedObstacleType;
    public int CircleLevel;
    public int GridIdx;
    public bool HaveCharacter;
    public GoalColor CharacterColor;
}

public enum GridType
{
    Empty,
    Normal,
    FixedPath,
    InteractablePath,
    FixedObstacle,
}

public enum FixedPathType
{
    Bridge,
}

public enum InteractablePathType
{
    Sandal,
}

public enum FixedObstacleType
{
    Fence,
}