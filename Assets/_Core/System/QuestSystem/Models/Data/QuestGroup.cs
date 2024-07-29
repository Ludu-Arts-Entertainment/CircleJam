using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGroup", menuName = "Quest/QuestGroup")]
public class QuestGroup : ScriptableObject
{
    public string Id;
    [OnValueChanged("OnQuestGroupEnumChanged")]
    public QuestGroupEnums QuestGroupEnum;
    public bool IsActive;
    public MetaData metaData;
    public QuestContainer QuestContainer;
    [BoxGroup("Config")]
    public int ActiveQuestCount;
    [BoxGroup("Config")]
    public bool IsCountedCompleted;
    [BoxGroup("Random Fill")]
    public bool IsFillWithRandom;
    [BoxGroup("Random Fill"),ShowIf("IsFillWithRandom")]
    public RandomQuestGeneratorType RandomQuestCreator = RandomQuestGeneratorType.None;
    [BoxGroup("Periodic")]
    public bool IsPeriodic;
    [BoxGroup("Periodic"),ShowIf("IsPeriodic")]
    public long PeriodTime;
    [BoxGroup("Periodic"),ShowIf("IsPeriodic")] 
    public int Order;
    [BoxGroup("Periodic"),ShowIf("IsPeriodic"),InfoBox("0 oldugu durumda persembeyi cumaya baglayan gece basliyor. her 1 arttırıldıgında bir gun ileriye kayar. 0-6 arası deger alır. 0=persembe, 1=cuma, 2=cumartesi, 3=pazar, 4=pazartesi, 5=sali, 6=carsamba")] 
    public int Shift;
    [BoxGroup("Periodic"),ShowIf("IsPeriodic"), OnValueChanged("OnQuestDeleteTimeChanged")] 
    public long LifeTime;
    [BoxGroup("Periodic"),ShowIf("IsPeriodic"), OnValueChanged("OnQuestDeleteTimeChanged")] 
    public long DeleteTime;
    
    private void OnQuestGroupEnumChanged()
    {
        switch (QuestGroupEnum)
        {
            case QuestGroupEnums.Daily:
                IsPeriodic = true;
                PeriodTime = 86400;
                LifeTime = 86400;
                break;
            case QuestGroupEnums.Weekly:
                IsPeriodic = true;
                PeriodTime = 604800;
                LifeTime = 604800;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void OnQuestDeleteTimeChanged()
    {
        if (DeleteTime < LifeTime)
        {
            DeleteTime = LifeTime;
        }
    }
}
