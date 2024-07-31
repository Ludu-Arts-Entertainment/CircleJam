using UnityEngine;

public class GridNode : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color normalEmisionColor, selectedEmisionColor;
    public int GridLevel => gridLevel;
    private int gridLevel;
    private bool haveCharacter;
    CharacterController character;

    private MaterialPropertyBlock PropertyBlock => propertyBlock??= new MaterialPropertyBlock();
    private MaterialPropertyBlock propertyBlock;

    public void Initialize(int gridLevel)
    {
        haveCharacter = false;
        character = null;
        
        this.gridLevel = gridLevel;
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
    }

    public Vector3 GetPosition()
    {
        return haveCharacter ? character.transform.position : transform.position;
    }

    public void SetSelectedColor(bool isSelected)
    {
        meshRenderer.GetPropertyBlock(PropertyBlock);
        PropertyBlock.SetColor("_EmissionColor", isSelected ? selectedEmisionColor : normalEmisionColor);
        meshRenderer.SetPropertyBlock(PropertyBlock);
    }
}