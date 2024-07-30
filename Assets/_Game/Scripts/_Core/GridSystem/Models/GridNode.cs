using UnityEngine;

public class GridNode : MonoBehaviour
{
    [SerializeField] private Collider collider;
    public int GridLevel => gridLevel;
    private int gridLevel;
    public void Initialize(int gridLevel)
    {
        this.gridLevel = gridLevel;
    }

    public void CreateCharacter(GoalColors color, Transform lookTarget)
    {
        var character = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<CharacterController>("Character", parent: transform);
        character.Initialize(color);
        character.transform.localPosition = collider.bounds.center;
        character.transform.localPosition = new Vector3(character.transform.localPosition.x, 0.5f, character.transform.localPosition.z);

        Vector3 direction = lookTarget.position - character.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        character.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}