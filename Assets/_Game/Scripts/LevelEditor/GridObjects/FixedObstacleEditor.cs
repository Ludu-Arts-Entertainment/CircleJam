using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FixedObstacleEditor : GridObjectEditor
{
    [SerializeField] public Dictionary<FixedObstacleType, List<GameObject>> objectByType;
    private GameObject createdObject;

    public void CreateObject(FixedObstacleType fixedObstacleType, int circleLevel)
    {
        if(!objectByType.ContainsKey(fixedObstacleType)) return;
        if(circleLevel > objectByType[fixedObstacleType].Count)
        {
            createdObject = Instantiate(objectByType[fixedObstacleType].First(), transform);
        }
        else
        {
            createdObject = Instantiate(objectByType[fixedObstacleType][circleLevel - 1], transform);
        }

        createdObject.transform.localPosition = Vector3.zero;
        createdObject.transform.localScale = Vector3.one;
        createdObject.transform.localRotation = Quaternion.identity;
        createdObject.name = fixedObstacleType.ToString();
    }

    public override void Remove()
    {
        DestroyImmediate(createdObject);
        base.Remove();
    }
}
