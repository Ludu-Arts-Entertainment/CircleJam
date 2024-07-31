using UnityEngine;

public class GridNode : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color normalEmisionColor, selectedEmisionColor;
    public int GridLevel => _gridLevel;
    private int _gridLevel;

    public bool HaveCharacter => haveCharacter;
    private bool haveCharacter;

    public CharacterController Character => character;
    private CharacterController character;

    private int _gridIdx;
    public int GridIdx => _gridIdx;

    private MaterialPropertyBlock PropertyBlock => propertyBlock??= new MaterialPropertyBlock();
    private MaterialPropertyBlock propertyBlock;

    public void Initialize(int gridLevel, int gridIdx)
    {
        haveCharacter = false;
        character = null;
        
        _gridLevel = gridLevel;
        _gridIdx = gridIdx;
    }

    public void UpdateGridIdx(int gridIdx)
    {
        _gridIdx = gridIdx;
    }

    public void CreateCharacter(GoalColors color, Transform lookTarget)
    {
        character = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<CharacterController>("Character", parent: transform);
        character.Initialize(color, this);
        character.transform.localPosition = collider.bounds.center;
        character.transform.localPosition = new Vector3(character.transform.localPosition.x, 0.5f, character.transform.localPosition.z);

        Vector3 direction = lookTarget.position - character.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        character.transform.rotation = Quaternion.Euler(0, angle, 0);

        haveCharacter = true;

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.CharacterCreated(color, character));
    }

    public void SetSelectedColor(bool isSelected)
    {
        meshRenderer.GetPropertyBlock(PropertyBlock);
        PropertyBlock.SetColor("_EmissionColor", isSelected ? selectedEmisionColor : normalEmisionColor);
        meshRenderer.SetPropertyBlock(PropertyBlock);
    }

    public void ResetGrid()
    {
        if(haveCharacter)
        {
            GameInstaller.Instance.SystemLocator.PoolManager.Destroy("Character", character);
        }
    }
}