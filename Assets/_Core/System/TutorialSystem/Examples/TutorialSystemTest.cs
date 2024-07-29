using UnityEngine;
using UnityEngine.UI;

public class TutorialSystemTest : MonoBehaviour
{
    [SerializeField] private Button button;


    [NaughtyAttributes.Button]
   public void TestButtonTutorial()
   {
        //TrackingService.Feed(TrackType.ShowButtonTutorial, button.GetComponent<RectTransform>());
   }

   public void CompleteTestTutorial()
   {
        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.TutorialCompleted(TutorialType.ShowButton));
   }
}
