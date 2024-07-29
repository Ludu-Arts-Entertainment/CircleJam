using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(FriendManagementTabToggle))]
public class FriendManagementTabToggleEditor : ToggleEditor {
    
    public override void OnInspectorGUI() {

        base.OnInspectorGUI();
        FriendManagementTabToggle component = (FriendManagementTabToggle)target;
        component._textMeshProUGUI = (TMPro.TextMeshProUGUI)EditorGUILayout.ObjectField("TextMeshProUGUI", component._textMeshProUGUI, typeof(TMPro.TextMeshProUGUI), true);
        component.friendsTabType = (FriendsTabType)EditorGUILayout.EnumPopup("Friend Tab Type", component.friendsTabType);
        component.textOffset = EditorGUILayout.FloatField("Text Offset", component.textOffset);
    }
}