using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SystemLocator
{
    private QuestManager _questManager;
    public QuestManager QuestManager => _questManager ??=
        GameInstaller.Instance.ManagerDictionary[ManagerEnums.QuestManager] as QuestManager;
}
