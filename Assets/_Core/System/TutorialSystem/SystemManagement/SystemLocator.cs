using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SystemLocator
{
    private TutorialManager _tutorialManager;
    public TutorialManager TutorialManager =>
        _tutorialManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.TutorialManager] as TutorialManager;
}
