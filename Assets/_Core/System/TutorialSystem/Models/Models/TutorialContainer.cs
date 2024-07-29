using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialContainer", menuName = "Tutorial/TutorialContainer")]
public class TutorialContainer : ScriptableObject
{
    public List<Tutorial> tutorials = new List<Tutorial>();
}