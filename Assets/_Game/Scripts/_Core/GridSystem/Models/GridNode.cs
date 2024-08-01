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

    private MeshRenderer meshRenderer;
    public void Initialize(GridNodeData gridNodeData)
    {
        character = null;
        _gridNodeData = gridNodeData;

        if(_gridNodeData.GridType == GridType.Normal)
        {
            var model = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>($"GridModel_{gridNodeData.CircleLevel + 1}", parent: transform);
            model.localPosition = Vector3.zero;
            model.localScale = Vector3.one;
            meshRenderer = model.GetComponentInChildren<MeshRenderer>();
        }
        if(_gridNodeData.GridType == GridType.FixedPath)
        {
            var modelName = GridNodeCollectionService.GetModelNameByFixedPathType(_gridNodeData.FixedPathType);
            var model = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>(modelName, parent: transform);
            model.transform.localPosition = collider.bounds.center;
            model.transform.localPosition = new Vector3(model.transform.localPosition.x, 0.5f, model.transform.localPosition.z);
        }
    }

    public void UpdateGridIdx(int gridIdx)
    {
        _gridNodeData.GridIdx = gridIdx;
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
        if(_gridNodeData.HaveCharacter)
        {
            GameInstaller.Instance.SystemLocator.PoolManager.Destroy("Character", character);
        }
    }
}

public class GridNodeData
{
    public GridType GridType;
    public FixedPathType FixedPathType;
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
}

public enum FixedPathType
{
    Bridge,
}